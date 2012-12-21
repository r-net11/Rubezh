using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Navigation;
using Infrastructure.Client;
using Microsoft.Practices.Prism.Events;
using FireMonitor.Multiclient.Events;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using System.Windows;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;

namespace FireMonitor.Multiclient.ViewModels
{
	public class MulticlientViewModel : ApplicationViewModel
	{
		private int _count;

		public MulticlientViewModel(int count)
		{
			_count = count;
			Title = "Multiclient FireSec-2";
			HideInTaskbar = false;
			AllowHelp = false;
			AllowMaximize = true;
			AllowMinimize = true;
			AllowClose = true;

			CreateToolbar();
		}
		private void CreateToolbar()
		{
			var menu = new MenuViewModel();
			for (int i = 0; i < _count; i++)
				menu.Items.Add(new HostViewModel(i));
			Toolbar = menu;
		}
	}
}
