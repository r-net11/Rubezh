using System;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using StrazhModule.Devices;

namespace StrazhModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public SKDDevice Device { get; private set; }
		public SKDDeviceState State
		{
			get { return Device.State; }
		}

		public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel(SKDDevice device)
		{
			Device = device;
			DeviceStateViewModel = new DeviceStateViewModel(State);
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			OpenForeverCommand = new RelayCommand(OnOpenForever, CanOpenForever);
			CloseForeverCommand = new RelayCommand(OnCloseForever, CanCloseForever);
			DeviceAccessStateNormalCommand = new RelayCommand(OnDeviceAccessStateNormal, CanDeviceAccessStateNormal);
			DeviceAccessStateCloseAlwaysCommand = new RelayCommand(OnDeviceAccessStateCloseAlways, CanDeviceAccessStateCloseAlways);
			DeviceAccessStateOpenAlwaysCommand = new RelayCommand(OnDeviceAccessStateOpenAlways, CanDeviceAccessStateOpenAlways);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal, CanShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowDoorCommand = new RelayCommand(OnShowDoor, CanShowDoor);

			if(Device.Zone != null)
			{
				Zone = new ZoneViewModel(Device.Zone);
			}

			IsEnabled = Device.IsEnabled;
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => DeviceStateViewModel);
		}

		public string PresentationAddress
		{
			get { return Device.Address; }
		}

		public bool IsEnabled { get; private set; }

		#region <Zone>

		public ZoneViewModel Zone { get; private set; }

		public bool HasZone
		{
			get { return Device.Zone != null; }
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>().Publish(Device.Zone.UID);
		}
		bool CanShowZone()
		{
			return Device.Zone != null;
		}

		#endregion </Zone>

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowSKDDevice(Device);
		}
		private bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowSKDDevice(Device);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDDevice = Device
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}
		private bool CanShowJournal()
		{
			return Device.IsRealDevice;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}
		public bool CanShowProperties()
		{
			return Device.IsRealDevice;
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			DeviceCommander.Open(Device);
		}
		bool CanOpen()
		{
			return DeviceCommander.CanOpen(Device);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			DeviceCommander.Close(Device);
		}
		bool CanClose()
		{
			return DeviceCommander.CanClose(Device);
		}

		public RelayCommand OpenForeverCommand { get; private set; }
		void OnOpenForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenDeviceForever(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpenForever()
		{
			return Device.DriverType == SKDDriverType.Lock && FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control) && State.StateClass != XStateClass.On && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseForeverCommand { get; private set; }
		void OnCloseForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseDeviceForever(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanCloseForever()
		{
			return Device.DriverType == SKDDriverType.Lock && FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand DeviceAccessStateNormalCommand { get; private set; }
		void OnDeviceAccessStateNormal()
		{
			DeviceCommander.SetAccessStateToNormal(Device);
		}
		bool CanDeviceAccessStateNormal()
		{
			return DeviceCommander.CanSetAccessStateToNormal(Device);
		}

		public RelayCommand DeviceAccessStateCloseAlwaysCommand { get; private set; }
		void OnDeviceAccessStateCloseAlways()
		{
			DeviceCommander.SetAccessStateToCloseAlways(Device);
		}
		bool CanDeviceAccessStateCloseAlways()
		{
			return DeviceCommander.CanSetAccessStateToCloseAlways(Device);
		}

		public RelayCommand DeviceAccessStateOpenAlwaysCommand { get; private set; }
		void OnDeviceAccessStateOpenAlways()
		{
			DeviceCommander.SetAccessStateToOpenAlways(Device);
		}
		bool CanDeviceAccessStateOpenAlways()
		{
			return DeviceCommander.CanSetAccessStateToOpenAlways(Device);
		}

		#region <Door>

		public SKDDoor Door
		{
			get { return Device.Door; }
		}

		public bool HasDoor
		{
			get { return Device.Door != null; }
		}

		public RelayCommand ShowDoorCommand { get; private set; }
		void OnShowDoor()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDoorEvent>().Publish(Device.Door.UID);
		}
		bool CanShowDoor()
		{
			return Device.Door != null;
		}

		#endregion </Door>
	}
}