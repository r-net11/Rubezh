using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public GKDevice Device { get; private set; }
		public string GkDescriptorName { get; private set; }
		public GKState State
		{
			get { return Device.State; }
		}

		public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel(GKDevice device)
		{
			Device = device;
			GkDescriptorName = Device.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType);
			DeviceStateViewModel = new DeviceStateViewModel(State, device.Driver.IsAm);
			State.StateChanged += OnStateChanged;
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
			OnPropertyChanged(() => IsStateImage);
			OnPropertyChanged(() => DeviceStateViewModel);
		}

		public string PresentationZone
		{
			get { return GKManager.GetPresentationZoneAndGuardZoneOrLogic(Device); }
		}

		public string PresentationLogic
		{
			get
			{
				return GKManager.GetPresentationLogic(Device.Logic);
			}
		}

		public bool IsStateImage
		{
			get
			{
				return State != null && (State.StateClass == XStateClass.Fire1 || State.StateClass == XStateClass.Fire2) &&
					   (Device.Driver.IsAm || Device.Children.Count > 0 &&
						!Device.AllChildren.Any(
							x => !x.Driver.IsAm && (x.State.StateClass == XStateClass.Fire1 || x.State.StateClass == XStateClass.Fire2)));
			}
		}

		public string PresentationNSLogic
		{
			get
			{
				return GKManager.GetPresentationLogic(Device.NSLogic);
			}
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (ShowOnPlanHelper.ShowObjectOnPlan(Device.PlanElementUIDs) && CanShowProperties())
				OnShowProperties();
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowObjectOnPlan(Device.PlanElementUIDs);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowOnPlan(Device.PlanElementUIDs);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Device != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Device.UID });
		}
		public bool CanShowJournal()
		{
			return Device.IsRealDevice;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (Device != null && Device.Driver != null && Device.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif)
				DialogService.ShowModalWindow(new PlotViewModel(Device));
			else
				DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}
		public bool CanShowProperties()
		{
			return Device.IsRealDevice || Device.DriverType == GKDriverType.RSR2_KAU_Shleif;
		}

		#region Ignore
		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(Device);
			}
		}
		bool CanSetIgnore()
		{
			return Device.AllParents.Any(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif) && Device.IsRealDevice &&
				!Device.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(Device);
			}
		}
		bool CanResetIgnore()
		{
			return Device.AllParents.Any(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif) && Device.IsRealDevice &&
				Device.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control);
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
						ClientManager.FiresecService.GKSetIgnoreRegime(device);
					}
				}
			}
		}
		bool CanSetIgnoreAll()
		{
			if (Device.DriverType == GKDriverType.RSR2_KAU_Shleif)
			{
				if (!ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
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
						ClientManager.FiresecService.GKSetAutomaticRegime(device);
					}
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			if (Device.DriverType == GKDriverType.RSR2_KAU_Shleif)
			{
				if (!ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
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