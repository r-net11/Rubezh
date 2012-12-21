using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Navigation;
using Infrastructure.Client;
using Microsoft.Practices.Prism.Events;
using FireMonitor.Multiclient.Events;

namespace FireMonitor.Multiclient.ViewModels
{
	public class MulticlientViewModel : ShellViewModel
	{
		private int _count;

		public MulticlientViewModel(int count)
		{
			_count = count;
			Title = "Multiclient FireSec-2";
			//Toolbar = new ToolbarViewModel();
			//ContentFotter = new UserFotterViewModel();
			Height = 700;
			Width = 1100;
			MinWidth = 800;
			MinHeight = 400;
			HideInTaskbar = false;
			AllowHelp = false;
			AllowMaximize = true;
			AllowMinimize = true;
			AllowClose = true;
			CreateNavigation();

			ServiceFactory.Events.GetEvent<ShowHostEvent>().Subscribe(arg => ((NavigationItem<ShowHostEvent, int>)NavigationItems[arg]).ShowViewPart(arg));
		}

		private void CreateNavigation()
		{
			NavigationItems = new List<NavigationItem>();
			for (int i = 0; i < _count; i++)
				NavigationItems.Add(new NavigationItem<ShowHostEvent, int>(new HostViewModel(i), "Экземпляр " + i.ToString(), null, null, null, i, false));
		}
	}
}
