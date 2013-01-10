using System.Collections.Generic;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Multiclient.ViewModels
{
	public class MulticlientViewModel : ApplicationViewModel
	{
		public MulticlientViewModel(int count)
		{
			Title = "Мультисерверная Оперативная Задача FireSec-2";
			HideInTaskbar = false;
			AllowHelp = false;
			AllowMaximize = true;
			AllowMinimize = true;
			AllowClose = true;
			Closed += (s, e) => App.Current.Shutdown();

			Hosts = new List<HostViewModel>();
			for (int i = 0; i < count; i++)
			{
				Hosts.Add(new HostViewModel(i));
			}
		}

		public List<HostViewModel> Hosts { get; private set; }

		HostViewModel _selectedHost;
		public HostViewModel SelectedHost
		{
			get { return _selectedHost; }
			set
			{
				_selectedHost = value;
				OnPropertyChanged("SelectedHost");
			}
		}
	}
}