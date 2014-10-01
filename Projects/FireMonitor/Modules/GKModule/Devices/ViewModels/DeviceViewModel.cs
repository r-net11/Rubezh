using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public XState State
		{
			get { return Device.State; }
		}

		public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel(XDevice device)
		{
			Device = device;
			DeviceStateViewModel = new DeviceStateViewModel(State);
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetIgnoreAllCommand = new RelayCommand(OnSetIgnoreAll, CanSetIgnoreAll);
			ResetIgnoreAllCommand = new RelayCommand(OnResetIgnoreAll, CanResetIgnoreAll);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal, CanShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => DeviceStateViewModel);

			if (Device.DriverType == XDriverType.MPT)
			{
				if (State.StateClass == XStateClass.TurningOn)
				{
					DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
				}
			}
		}

		public string PresentationZone
		{
			get { return XManager.GetPresentationZone(Device); }
		}

		public string PresentationLogic
		{
			get
			{
				if (Device.Driver.HasLogic)
					return XManager.GetPresentationZone(Device);
				return null;
			}
		}

		public string PresentationZoneWithNS
		{
			get
			{
				if (Device.DriverType == XDriverType.FirePump)
					return XManager.GetPresentationZone(Device.NSLogic);
				return XManager.GetPresentationZone(Device);
			}
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (CanShowOnPlan())
				ShowOnPlanHelper.ShowDevice(Device);
			else if (CanShowProperties())
				DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
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
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKDevice = Device
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}
		public bool CanShowJournal()
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

		#region Ignore
		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKSetIgnoreRegime(Device);
			}
		}
		bool CanSetIgnore()
		{
			return Device.AllParents.Any(x => x.DriverType == XDriverType.KAU_Shleif || x.DriverType == XDriverType.RSR2_KAU_Shleif) && Device.IsRealDevice &&
				!Device.State.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKSetAutomaticRegime(Device);
			}
		}
		bool CanResetIgnore()
		{
			return Device.AllParents.Any(x => x.DriverType == XDriverType.KAU_Shleif || x.DriverType == XDriverType.RSR2_KAU_Shleif) && Device.IsRealDevice &&
				Device.State.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}
		#endregion

		#region IgnoreAll
		public RelayCommand SetIgnoreAllCommand { get; private set; }
		void OnSetIgnoreAll()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				foreach (var device in Device.AllChildrenAndSelf)
				{
					if (device.IsRealDevice && !device.State.StateClasses.Contains(XStateClass.Ignore))
					{
						FiresecManager.FiresecService.GKSetIgnoreRegime(device);
					}
				}
			}
		}
		bool CanSetIgnoreAll()
		{
			if (Device.DriverType == XDriverType.KAU_Shleif || Device.DriverType == XDriverType.RSR2_KAU_Shleif)
			{
				if (!FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices))
					return false;
				foreach (var device in Device.AllChildrenAndSelf)
				{
					if (device.IsRealDevice && !device.State.StateClasses.Contains(XStateClass.Ignore))
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
				foreach (var device in Device.AllChildrenAndSelf)
				{
					if (device.IsRealDevice && device.State.StateClasses.Contains(XStateClass.Ignore))
					{
						FiresecManager.FiresecService.GKSetAutomaticRegime(device);
					}
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			if (Device.DriverType == XDriverType.KAU_Shleif || Device.DriverType == XDriverType.RSR2_KAU_Shleif)
			{
				if (!FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices))
					return false;
				foreach (var device in Device.AllChildrenAndSelf)
				{
					if (device.IsRealDevice && device.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}
			}
			return false;
		}
		#endregion

		public bool IsBold { get; set; }
	}
}