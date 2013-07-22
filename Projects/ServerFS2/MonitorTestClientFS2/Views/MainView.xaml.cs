using System.Windows;
using System;
using System.Windows.Interop;
using ServerFS2;
using System.Windows.Forms;

namespace MonitorClientFS2
{
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
		}

		//protected override void OnSourceInitialized(EventArgs e)
		//{
		//    base.OnSourceInitialized(e);
		//    HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
		//    source.AddHook(WndProc);
		//}

		//private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		//{
		//    var message = Message.Create(hwnd, msg, wParam, lParam);
		//    USBManager.ParseWndMessage(ref message);
		//    return IntPtr.Zero;
		//}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var dataContext = this.DataContext as MainViewModel;
			if (dataContext.StopMonitoringCommand.CanExecute(null))
				dataContext.StopMonitoringCommand.Execute();
		}
	}
}