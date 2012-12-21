using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using System.Windows.Input;
using System.Windows;

namespace FireMonitor.Multiclient.ViewModels
{
	public class HostViewModel : MenuButtonViewModel
	{
		public int Index { get; private set; }
		private MulticlientControllerWrapper _controller;
		private Window _win;

		public HostViewModel(int index)
			: base(null, "/Controls;component/Images/Maximize.png", index.ToString())
		{
			Index = index;
			Command = new RelayCommand(OnClick, CanClick);
			_controller = new MulticlientControllerWrapper(index);
			_controller.ControlChanged += new EventHandler(ControlChanged);
			_controller.Start();
		}

		private void ControlChanged(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}
		private void OnClick()
		{
			if (_win == null)
			{
				var content = _controller.GetContent();
				_win = new Window();
				_win.Closed += (s, e) => { _win.Content = null; _win = null; };
				_win.Content = content;
				_win.Show();
				_win.Activate();
			}
			else
			{
				if (_win.WindowState == WindowState.Minimized)
					_win.WindowState = WindowState.Normal;
				_win.Activate();
			}
		}
		private bool CanClick()
		{
			return _controller.Contract != null;
		}
	}
}
