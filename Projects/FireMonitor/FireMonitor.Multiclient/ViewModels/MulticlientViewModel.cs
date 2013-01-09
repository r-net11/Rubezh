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
			//CreateToolbar();
			Closed += (s, e) => App.Current.Shutdown();

			Hosts = new List<HostViewModel>();
			for (int i = 0; i < count; i++)
			{
				Hosts.Add(new HostViewModel(i));
			}
		}
		//private void CreateToolbar()
		//{
		//    ShowHost = new RelayCommand<int?>(OnShowHost, CanShowHost);
		//    var menu = new MenuViewModel();
		//    Hosts = new List<HostViewModel>();
		//    for (int i = 0; i < _count; i++)
		//    {
		//        menu.Items.Add(new MenuButtonViewModel(ShowHost, "/Controls;component/Images/Maximize.png", i.ToString(), i));
		//        Hosts.Add(new HostViewModel(i));
		//    }
		//    Toolbar = menu;
		//}

		public RelayCommand<int?> ShowHost;
		//private void OnShowHost(int? index)
		//{
		//    SelectedHost = index.HasValue ? Hosts[index.Value] : null;
		//    OnPropertyChanged(() => SelectedHost);
		//}
		//private bool CanShowHost(int? index)
		//{
		//    return index.HasValue ? Hosts[index.Value].IsReady : false;
		//}

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