using System;
using System.Windows;
using System.Windows.Input;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace FireMonitor.Multiclient.ViewModels
{
	public class HostViewModel : BaseViewModel
	{
		public int Index { get; private set; }
		private MulticlientControllerWrapper _controller;

		public HostViewModel(int index)
			: base()
		{
			Index = index;
			_controller = new MulticlientControllerWrapper(index);
			_controller.ControlChanged += new EventHandler(ControlChanged);
			_controller.Start();
		}

		public bool IsReady
		{
			get { return _controller.Contract != null; }
		}

		private void ControlChanged(object sender, EventArgs e)
		{
			ApplicationService.Invoke(() =>
				{
					CommandManager.InvalidateRequerySuggested();
					HostControl = _controller.GetContent();
					OnPropertyChanged(() => HostControl);
				});
		}

		public FrameworkElement HostControl { get; private set; }
		public string Caption
		{
			get { return _controller.AppDomain.FriendlyName; }
		}

		//private Window _win;
		//private void OnClick()
		//{
		//    if (_win == null)
		//    {
		//        var content = _controller.GetContent();
		//        _win = new Window();
		//        _win.SetResourceReference(Window.BackgroundProperty, "BaseWindowBackgroundBrush");
		//        _win.Closed += (s, e) => { _win.Content = null; _win = null; };
		//        _win.Content = content;
		//        _win.Show();
		//        _win.Activate();
		//    }
		//    else
		//    {
		//        if (_win.WindowState == WindowState.Minimized)
		//            _win.WindowState = WindowState.Normal;
		//        _win.Activate();
		//    }
		//}
		//private bool CanClick()
		//{
		//    return _controller.Contract != null;
		//}
	}
}