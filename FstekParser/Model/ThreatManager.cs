using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace FstekParser
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
                    MessageBox.Show("Тут пока ничего нет, но можно загрузить).\nНажмите 'Обновить таблицу' для первичной загрузки данных");
                    return;
                }
                catch (JsonReaderException)
                {
                    MessageBox.Show("Серверное хранилище повреждено. Попробуйте обновить таблицу");
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
                    var newThreats = ThreatDatabase.Instance.UpdateThreats();
                    Threats = newThreats;
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
    }
}