using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Microsoft.Win32;

namespace AutomationModule.ViewModels
{
	public class OPCServersViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static OPCServersViewModel Current { get; private set; }
		public OPCServersViewModel()
		{
			Current = this;
			Menu = new OPCServersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		public void Initialize()
		{
			OPCServers = new ObservableCollection<OPCServerViewModel>();
			foreach (var opcServer in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.OPCServers)
			{
				var opcServerViewModel = new OPCServerViewModel(opcServer);
				OPCServers.Add(opcServerViewModel);
			}
			SelectedOPCServer = OPCServers.FirstOrDefault();
		}

		ObservableCollection<OPCServerViewModel> _opcServers;
		public ObservableCollection<OPCServerViewModel> OPCServers
		{
			get { return _opcServers; }
			set
			{
				_opcServers = value;
				OnPropertyChanged(() => OPCServers);
			}
		}

		OPCServerViewModel _selectedOPCServer;
		public OPCServerViewModel SelectedOPCServer
		{
			get { return _selectedOPCServer; }
			set
			{
				_selectedOPCServer = value;
				OnPropertyChanged(() => SelectedOPCServer);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var opcServerDetailsViewModel = new OPCServerDetailsViewModel();
			if (DialogService.ShowModalWindow(opcServerDetailsViewModel))
			{
				var opcServerViewModel = new OPCServerViewModel(opcServerDetailsViewModel.OPCServer);
				OPCServers.Add(opcServerViewModel);
				SelectedOPCServer = OPCServers.FirstOrDefault();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = OPCServers.IndexOf(SelectedOPCServer);
			FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.OPCServers.Remove(SelectedOPCServer.OPCServer);
			OPCServers.Remove(SelectedOPCServer);
			index = Math.Min(index, OPCServers.Count - 1);
			if (index > -1)
				SelectedOPCServer = OPCServers[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var opcServerDetailsViewModel = new OPCServerDetailsViewModel(SelectedOPCServer.OPCServer);
			if (DialogService.ShowModalWindow(opcServerDetailsViewModel))
			{
				SelectedOPCServer.OPCServer = opcServerDetailsViewModel.OPCServer;
				SelectedOPCServer.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedOPCServer != null;
		}

		public void Select(Guid opcServerUID)
		{
			if (opcServerUID != Guid.Empty)
			{
				SelectedOPCServer = OPCServers.FirstOrDefault(item => item.OPCServer.Uid == opcServerUID);
			}
		}
	}
}