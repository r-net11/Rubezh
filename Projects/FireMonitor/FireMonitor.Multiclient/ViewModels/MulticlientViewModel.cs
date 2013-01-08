using System.Collections.Generic;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Multiclient.ViewModels
{
	public class MulticlientViewModel : ApplicationViewModel
	{
		private int _count;
		private List<HostViewModel> _hosts;

		public MulticlientViewModel(int count)
		{
			_count = count;
			Title = "Мультисерверная Оперативная Задача FireSec-2";
			HideInTaskbar = false;
			AllowHelp = false;
			AllowMaximize = true;
			AllowMinimize = true;
			AllowClose = true;
			CreateToolbar();
			Closed += (s, e) => App.Current.Shutdown();
		}
		private void CreateToolbar()
		{
			ShowHost = new RelayCommand<int?>(OnShowHost, CanShowHost);
			var menu = new MenuViewModel();
			_hosts = new List<HostViewModel>();
			for (int i = 0; i < _count; i++)
			{
				menu.Items.Add(new MenuButtonViewModel(ShowHost, "/Controls;component/Images/Maximize.png", i.ToString(), i));
				_hosts.Add(new HostViewModel(i));
			}
			Toolbar = menu;
		}

		public RelayCommand<int?> ShowHost;
		private void OnShowHost(int? index)
		{
			SelectedHost = index.HasValue ? _hosts[index.Value] : null;
			OnPropertyChanged(() => SelectedHost);
		}
		private bool CanShowHost(int? index)
		{
			return index.HasValue ? _hosts[index.Value].IsReady : false;
		}

		public HostViewModel SelectedHost { get; set; }
	}
}