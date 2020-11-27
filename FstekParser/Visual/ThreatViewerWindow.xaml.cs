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
    /// Interaction logic for ThreatViewerWindow.xaml
    /// </summary>
    public partial class ThreatViewerWindow : Window
    {
        private readonly List<Threat> threats;
        public bool IsTerminated { get; private set; } = true;

        private int pos;

        public int Position
        {
            get => pos;
            set
            {
                if (pos < 0 || pos >= threats.Count)
                    throw new ArgumentOutOfRangeException($"pos = {value}");

                pos = value;
                Update();
            }
        }

        protected ThreatViewerWindow()
        {
            InitializeComponent();

            Button_ToThreatList.Click += (o, args) =>
            {
                IsTerminated = false;
                Close();
            };

            Button_Prev.Click += (o, args) =>
            {
                if (pos > 0)
                {
                    --Position;
                }
            };

            Button_Next.Click += (o, args) =>
            {
                if (pos < threats.Count - 1)
                {
                    ++Position;
                }
            };
        }

        public ThreatViewerWindow(List<Threat> threats) : this()
        {
            this.threats = threats;
        }

        private void Update()
        {
            Clear();

            var t = threats[Position];
            Id.Inlines.Add($"УБИ. {t.Id:D3}");
            Name.Inlines.Add(t.Name);
            Description.Inlines.Add(t.Description);
            Source.Inlines.Add(t.Source);
            Target.Inlines.Add(t.Target);

            if (t.ConfidenceViolation)
            {
                ViolationsList.ListItems.Add(Item_ConfidenceViolation);
            }

            if (t.IntegrityViolation)
            {
                ViolationsList.ListItems.Add(Item_IntegrityViolation);
            }

            if (t.AccessViolation)
            {
                ViolationsList.ListItems.Add(Item_AccessViolation);
            }

            Button_Prev.IsEnabled = pos > 0;
            Button_Next.IsEnabled = pos < threats.Count - 1;
        }

        private void Clear()
        {
            Id.Inlines.Clear();
            Name.Inlines.Clear();
            Description.Inlines.Clear();
            Source.Inlines.Clear();
            Target.Inlines.Clear();
            ViolationsList.ListItems.Clear();
        }
    }
}