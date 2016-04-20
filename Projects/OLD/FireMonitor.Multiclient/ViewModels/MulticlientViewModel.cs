using System;
using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using MuliclientAPI;

namespace FireMonitor.Multiclient.ViewModels
{
	public class MulticlientViewModel : ApplicationViewModel
	{
		public MulticlientViewModel()
		{
			Title = "Мультисерверная Оперативная Задача FireSec";
			HideInTaskbar = false;
			AllowHelp = false;
			AllowMaximize = true;
			AllowMinimize = true;
			AllowClose = true;
			Closed += (s, e) => App.Current.Shutdown();
			Hosts = new ObservableCollection<HostViewModel>();
		}

		public void Initialize(MulticlientConfiguration multiclientConfiguration)
		{
			if (multiclientConfiguration != null)
			{
				int index = 0;
				foreach (var multiclientData in multiclientConfiguration.MulticlientDatas)
				{
					if (!multiclientData.IsNotUsed)
					{
						multiclientData.Id = index++.ToString();
						var hostViewModel = new HostViewModel(multiclientData);
						hostViewModel.StateTypeChanged += new Action<HostViewModel>(OnStateTypeChanged);
						Hosts.Add(hostViewModel);
					}
				}
			}
		}

		public ObservableCollection<HostViewModel> Hosts { get; private set; }

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

		void OnStateTypeChanged(HostViewModel sourceHostViewModel)
		{
			if (SelectedHost != null && (SelectedHost.StateType == StateType.Fire || SelectedHost.StateType == StateType.Attention))
				return;
			SelectedHost = sourceHostViewModel;
			Dispatcher.Invoke(new Action(() =>
			{
				ActivateWindow();
			}));
		}

		void ActivateWindow()
		{
			if ((App.Current.MainWindow != null) && (!App.Current.MainWindow.IsActive))
			{
				App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
				App.Current.MainWindow.Activate();
				App.Current.MainWindow.BringIntoView();
				App.Current.MainWindow.Focus();
				App.Current.MainWindow.Show();
				App.Current.MainWindow.BringIntoView();
			}
		}
	}
}