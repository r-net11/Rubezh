using System;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using XFiresecAPI;
using Controls.Converters;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel(XDevice device)
		{
			Device = device;
			DeviceState = Device.DeviceState;
			DeviceStateViewModel = new DeviceStateViewModel(DeviceState);
			DeviceState.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			DeviceCommandsViewModel = new DeviceCommandsViewModel(DeviceState);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetIgnoreAllCommand = new RelayCommand(OnSetIgnoreAll, CanSetIgnoreAll);
			ResetIgnoreAllCommand = new RelayCommand(OnResetIgnoreAll, CanResetIgnoreAll);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal, CanShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
		}

		void OnStateChanged()
		{
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("DeviceStateViewModel");

			if (Device.DriverType == XDriverType.MPT)
			{
				if (DeviceState.StateClass == XStateClass.TurningOn)
				{
					ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Device.UID);
				}
			}
		}

		public string PresentationZone
		{
			get { return XManager.GetPresentationZone(Device); }
		}

		public string PresentationZoneWithNS
		{
			get
			{
				if (Device.DriverType == XDriverType.Pump || Device.DriverType == XDriverType.RSR2_Bush)
					return XManager.GetPresentationZone(Device.NSLogic);
				return XManager.GetPresentationZone(Device);
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDevice(Device);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowDevice(Device);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Device = Device
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}
		public bool CanShowJournal()
		{
			return Device.IsRealDevice;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Device.UID);
		}
		public bool CanShowProperties()
		{
			return Device.IsRealDevice;
		}

		#region Ignore
		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			ObjectCommandSendHelper.SetIgnoreRegime(Device);
		}
		bool CanSetIgnore()
		{
			return Device.IsRealDevice && !Device.DeviceState.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			ObjectCommandSendHelper.SetAutomaticRegime(Device);
		}
		bool CanResetIgnore()
		{
			return Device.IsRealDevice && Device.DeviceState.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}
		#endregion

		#region IgnoreAll
		public RelayCommand SetIgnoreAllCommand { get; private set; }
		void OnSetIgnoreAll()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var devices = XManager.GetAllDeviceChildren(Device);
				foreach (var device in devices)
				{
					if (device.IsRealDevice && !device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
					{
						ObjectCommandSendHelper.SetIgnoreRegime(device, false);
					}
				}
			}
		}
		bool CanSetIgnoreAll()
		{
			if (Device.DriverType == XDriverType.KAU_Shleif || Device.DriverType == XDriverType.RSR2_KAU_Shleif)
			{
				if (!FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList))
					return false;
				var devices = XManager.GetAllDeviceChildren(Device);
				foreach (var device in devices)
				{
					if (device.IsRealDevice && !device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}
			}
			return false;
		}

		public RelayCommand ResetIgnoreAllCommand { get; private set; }
		void OnResetIgnoreAll()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var devices = XManager.GetAllDeviceChildren(Device);
				foreach (var device in devices)
				{
					if (device.IsRealDevice && device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
					{
						ObjectCommandSendHelper.SetAutomaticRegime(device, false);
					}
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			if (Device.DriverType == XDriverType.KAU_Shleif || Device.DriverType == XDriverType.RSR2_KAU_Shleif)
			{
				if (!FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList))
					return false;
				var devices = XManager.GetAllDeviceChildren(Device);
				foreach (var device in devices)
				{
					if (device.IsRealDevice && device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}
			}
			return false;
		}
		#endregion

		public bool IsBold { get; set; }
	}
}