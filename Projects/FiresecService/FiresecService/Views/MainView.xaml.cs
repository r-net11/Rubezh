using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace FiresecService.Views
{
    public partial class MainView : UserControl
    {
        public static MainView Current { get; private set; }

        public MainView()
        {
            InitializeComponent();
            Current = this;
        }

        public static void AddMessage(string message)
        {
            Current._textBox.AppendText(message);
            Current._textBox.AppendText(Environment.NewLine);
        }

        public static void SetStatus(string message)
        {
            Current.SetStatusMessage(message);
        }

        void SetStatusMessage(string message)
        {
            return;
            Trace.WriteLine(message);

            _statusTextBlock.Text = message;
            _statusTextBlock.InvalidateVisual();

            this.Dispatcher.Invoke(new Action(() => _statusTextBlock.Text = message), null);
        }
    }
}