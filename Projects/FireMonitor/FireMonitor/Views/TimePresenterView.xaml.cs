using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FireMonitor
{
    public partial class TimePresenterView : UserControl
    {
        DispatcherTimer dispatcherTimer;

        public TimePresenterView()
        {
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _textBlock.Text = DateTime.Now.ToString();
        }
    }
}