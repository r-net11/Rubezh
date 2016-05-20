using System.Windows.Controls;
using System.Windows.Threading;
using System;

namespace RubezhMonitor.Views
{
	public partial class ToolbarView : UserControl
	{
		DispatcherTimer _dispatcherTimer;
		
		public ToolbarView()
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