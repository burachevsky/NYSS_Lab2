using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FstekParser
{
    /// <summary>
    /// Interaction logic for DifferenceListWindow.xaml
    /// </summary>
    public partial class DifferenceListWindow : Window
    {
        protected DifferenceListWindow()
        {
            InitializeComponent();
        }

        public DifferenceListWindow(List<Difference> differences) : this()
        {
            ListView_Differences.ItemsSource = differences;
            ListView_Differences.MouseDoubleClick += (sender, a) =>
            {
                var listView = sender as ListView;
                var index = listView.SelectedIndex;
                if (index >= 0 && index <= differences.Count)
                {
                    var difference = differences[index];
                    
                    var list = new List<Threat>();
                    
                    if (difference.OldThreat != null)
                    {
                        list.Add(difference.OldThreat);
                    }
                    
                    if (difference.NewThreat != null)
                    {
                        list.Add(difference.NewThreat);
                    }

                    var threatViewerWindow = new ThreatViewerWindow(list);
                    
                    threatViewerWindow.Button_Next.Content = "Новая версия";
                    threatViewerWindow.Button_Prev.Content = "Старая версия";
                    
                    if (difference.OldThreat == null || difference.NewThreat == null)
                    {
                        threatViewerWindow.Button_Next.Visibility = Visibility.Hidden;
                        threatViewerWindow.Button_Prev.Visibility = Visibility.Hidden;
                    }

                    threatViewerWindow.Position = 0;
                    threatViewerWindow.ShowDialog();
                }
            };
        }
    }
}
