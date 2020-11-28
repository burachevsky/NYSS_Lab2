using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using FstekParser.Visual;
using Newtonsoft.Json;

namespace FstekParser.Model
{
    public class ThreatManager
    {
        private readonly MainWindow MainWindow;

        private List<Threat> threats;

        private List<Threat> Threats
        {
            get { return threats; }
            set
            {
                threats = value;
                MainWindow.Threats = value;
            }
        }

        public ThreatManager(MainWindow mainWindow)
        {
            this.MainWindow = mainWindow;
        }

        public void InitThreats()
        {
            RunAsync(() =>
            {
                List<Threat> threats;

                try
                {
                    threats = ThreatDatabase.Instance.GetThreats();
                    if (threats == null)
                    {
                        MessageBox.Show("Серверное хранилище повреждено. Попробуйте обновить таблицу");
                    }

                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(
                        "Тут пока ничего нет, но можно загрузить).\nНажмите 'Обновить таблицу' для первичной загрузки данных");
                    return;
                }
                catch (JsonReaderException)
                {
                    MessageBox.Show("Локальное хранилище повреждено. Попробуйте обновить таблицу");
                    return;
                }
                catch (JsonSerializationException)
                {
                    MessageBox.Show("Локальное хранилище повреждено. Попробуйте обновить таблицу");
                    return;
                }

                Threats = threats;
            });
        }

        public void RefreshDatabase()
        {
            RunAsync(() =>
            {
                try
                {
                    MainWindow.Button_refresh.IsEnabled = false;
                    var oldThreats = Threats;
                    try
                    {
                        var newThreats = ThreatDatabase.Instance.UpdateThreats();
                        Threats = newThreats;

                        if (newThreats != null && newThreats.Count > 0)
                        {
                            if (oldThreats != null)
                            {
                                var differences = CountDifference(oldThreats, newThreats);

                                if (differences.Count == 0)
                                {
                                    MessageBox.Show("Данные успешно обновлены.\nИзменений не обнаружено.");
                                }
                                else
                                {
                                    new DifferenceListWindow(differences).ShowDialog();
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Данные успешно загружены и теперь хранятся в C:\\FstekParser\\threats.JSON\nКоличество элементов: {newThreats.Count}");
                            }
                        }
                        else
                        {
                            MessageBox.Show("При обновлении данных произошла ошибка:");
                        }

                        
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("При обновлении данных произошла ошибка:\n" + e.GetType() + e.StackTrace);
                    }
                }
                finally
                {
                    MainWindow.Button_refresh.IsEnabled = true;
                }
            });
        }

        public void OpenThreatViewer(int pos)
        {
            var threatViewer = new ThreatViewerWindow(Threats);

            threatViewer.Left = MainWindow.Left;
            threatViewer.Top = MainWindow.Top;
            threatViewer.Height = MainWindow.Height;
            threatViewer.Width = MainWindow.Width;
            threatViewer.WindowState = MainWindow.WindowState;

            threatViewer.Position = pos;

            threatViewer.Closed += (o, args) =>
            {
                if (threatViewer.IsTerminated)
                {
                    Application.Current.Shutdown();
                }

                try
                {
                    MainWindow.Left = threatViewer.Left;
                    MainWindow.Top = threatViewer.Top;
                    MainWindow.Height = threatViewer.Height;
                    MainWindow.Width = threatViewer.Width;
                    MainWindow.Visibility = Visibility.Visible;
                    MainWindow.WindowState = threatViewer.WindowState;
                }
                catch (Exception e)
                {
                    MainWindow.HandleException(e);
                }
            };

            MainWindow.Visibility = Visibility.Hidden;

            threatViewer.Show();
        }

        public void RunAsync(Action action)
        {
            MainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    MainWindow.HandleException(e);
                }
            }));
        }

        private List<Difference> CountDifference(List<Threat> oldThreats, List<Threat> newThreats)
        {
            oldThreats.Sort();
            newThreats.Sort();

            var oldEn = oldThreats.GetEnumerator();
            var newEn = newThreats.GetEnumerator();

            var differenceList = new List<Difference>();

            while (true)
            {
                var oldHasNext = oldEn.MoveNext();
                var newHasNext = newEn.MoveNext();

                Threat o;
                Threat n;

                if (!(oldHasNext || newHasNext))
                {
                    break;
                }
                
                if (!oldHasNext)
                {
                    do
                    {
                        n = newEn.Current;
                        differenceList.Add(new Difference(n.Id, null, n) );

                    } while (newEn.MoveNext());

                    break;
                }
                
                if (!newHasNext)
                {
                    do
                    {
                        o = oldEn.Current;
                        differenceList.Add(new Difference(o.Id, o, null) );

                    } while (oldEn.MoveNext());

                    break;
                }

                o = oldEn.Current;
                n = newEn.Current;

                if (o.Equals(n))
                {
                    continue;
                }


                if (o.Id == n.Id)
                {
                    differenceList.Add(new Difference(o.Id, o, n));
                    continue;
                }
                
                if (o > n)
                {
                    do
                    {
                        n = newEn.Current;
                        
                        if (o <= n)
                        {
                            break;
                        }

                        differenceList.Add(new Difference(n.Id, null, n) );
                        
                    } while (newEn.MoveNext());
                }
                else
                {
                    do
                    {
                        o = oldEn.Current;
                        
                        if (o >= n)
                        {
                            break;
                        }

                        differenceList.Add(new Difference(o.Id, o, null) );

                    } while (oldEn.MoveNext());
                }

                if (!oldEn.Current.Equals(newEn.Current))
                {
                    differenceList.Add(new Difference(oldEn.Current.Id, oldEn.Current, newEn.Current));
                }
            }

            return differenceList.Distinct().ToList();
        }

        
    }
}