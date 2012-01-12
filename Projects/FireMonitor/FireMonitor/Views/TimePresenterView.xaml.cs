using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FireMonitor.Views
{
    public partial class TimePresenterView : UserControl
    {
        DispatcherTimer _dispatcherTimer;

        public TimePresenterView()
        {
            InitializeComponent();

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _textBlock.Text = DateTime.Now.ToString();
        }
    }
}