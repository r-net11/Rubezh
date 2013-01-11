using System.Collections.Generic;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using FiresecAPI;
using MuliclientAPI;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;

namespace FireMonitor.Multiclient.ViewModels
{
	public class MulticlientViewModel : ApplicationViewModel
	{
		public MulticlientViewModel()
		{
			Title = "Мультисерверная Оперативная Задача FireSec-2";
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
			foreach (var multiclientData in multiclientConfiguration.MulticlientDatas)
			{
				multiclientData.Id = Guid.NewGuid().ToString();
				var hostViewModel = new HostViewModel(multiclientData);
				hostViewModel.StateTypeChanged += new Action(OnStateTypeChanged);
				Hosts.Add(hostViewModel);
			}
		}

		StateType MainStateType = StateType.Norm;

		void OnStateTypeChanged()
		{
			var newStateType = StateType.Norm;
			HostViewModel newHostViewModel = null;
			foreach (var hostViewModel in Hosts)
			{
				if (hostViewModel.StateType < newStateType)
				{
					newStateType = hostViewModel.StateType;
					newHostViewModel = hostViewModel;
				}
			}
			if (newStateType != MainStateType)
			{
				SelectedHost = newHostViewModel;
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
	}
}