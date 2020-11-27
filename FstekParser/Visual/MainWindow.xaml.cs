using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FstekParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Threat> threats;

        public List<Threat> Threats
        {
            get { return threats; }

            set
            {
                threats = value;
                ResetPage();
            }
        }

        private int position = 0;
        private int limit = 15;

        public ThreatManager ThreatManager { get; }

        public MainWindow()
        {
            InitializeComponent();
            ThreatManager = new ThreatManager(this);
            ThreatManager.InitThreats();

            ComboBox_pages.SelectionChanged += (o, a) => { ResetPage(); };

            Button_refresh.Click += (o, a) =>
            {
                ThreatManager.RefreshDatabase();
            };

            Button_right.Click += (o, a) =>
            {
                ShiftRight();
                UpdatePage();
            };

            Button_left.Click += (o, a) =>
            {
                ShiftLeft();
                UpdatePage();
            };

            Button_left.IsEnabled = false;
            Button_right.IsEnabled = false;
        }

        public static void HandleException(Exception e)
        {
            MessageBox.Show($"Упс.. Где-то протечка..\n{e.GetType()} {e.StackTrace}");
        }

        private void ThreatsList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var listView = sender as ListView;
                var threatIndex = listView.SelectedIndex;

                if (threatIndex >= 0 && threatIndex < threats.Count)
                    ThreatManager.OpenThreatViewer(threatIndex);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void ShiftRight()
        {
            var shift = CurrentShift();
            if (limit + shift <= threats.Count)
            {
                position = limit;
                limit += shift;
            }
            else
            {
                position = limit;
                limit = threats.Count;
            }
        }

        private void ResetPage()
        {
            position = 0;
            limit = CurrentShift();

            UpdatePage();
        }

        private void ShiftLeft()
        {
            var shift = CurrentShift();
            if (position - shift >= 0)
            {
                limit = position;
                position -= shift;
            }
            else
            {
                position = 0;
                limit = position + shift;
            }
        }

        private void UpdatePage()
        {
            ThreatManager.RunAsync(() =>
            {
                if (threats == null)
                {
                    return;
                }

                int cap = threats.Count;
                int pos = position >= 0 ? position : 0;
                int lim = limit <= cap ? limit : cap;
                Label_pages.Content = $"{pos + 1} - {lim} из {threats.Count}";
                threatsList.ItemsSource = threats.GetRange(pos, lim - pos);
                Button_left.IsEnabled = pos > 0;
                Button_right.IsEnabled = lim < threats.Count;
            });
        }

        private int CurrentShift()
        {
            return int.Parse((ComboBox_pages.SelectedItem as ComboBoxItem).Content.ToString());
        }
    }
}