using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace GKModule.ViewModels
{
	public class AlarmViewModel : BaseViewModel
	{
		public Alarm Alarm { get; private set; }

		public AlarmViewModel(Alarm alarm)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			TurnOnAutomaticCommand = new RelayCommand(OnTurnOnAutomatic, CanTurnOnAutomatic);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			Alarm = alarm;
			InitializePlans();
		}

		public string ObjectName
		{
			get
			{
				if (Alarm.GkBaseEntity != null)
					return Alarm.GkBaseEntity.PresentationName;
				return null;
			}
		}

		public string ImageSource
		{
			get
			{
				var device = Alarm.GkBaseEntity as GKDevice;
				if ( device != null)
					return device.Driver.ImageSource;
				if (Alarm.GkBaseEntity as GKZone != null)
					return "/Controls;component/Images/Zone.png";
				if (Alarm.GkBaseEntity as GKGuardZone != null)
					return "/Controls;component/Images/GuardZone.png";
				if (Alarm.GkBaseEntity as GKDirection != null)
					return "/Controls;component/Images/Blue_Direction.png";
				if (Alarm.GkBaseEntity as GKDoor != null)
					return "/Controls;component/Images/Door.png";
				if (Alarm.GkBaseEntity as GKDelay != null)
					return "/Controls;component/Images/Delay.png";
				if (Alarm.GkBaseEntity as GKMPT != null)
					return "/Controls;component/Images/BMpt.png";
				return null;
			}
		}

		public XStateClass ObjectStateClass
		{
			get
			{
				if (Alarm.GkBaseEntity != null)
					return Alarm.GkBaseEntity.State.StateClass;
				return XStateClass.Norm;
			}
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();

			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase;
				var elementUnion = plan.ElementUnion;
				if (Alarm.GkBaseEntity != null)
				{
					elementBase = elementUnion.FirstOrDefault(x => x.UID == Alarm.GkBaseEntity.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.GkBaseEntity = Alarm.GkBaseEntity;
						Plans.Add(alarmPlanViewModel);
					}
				}
			}
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else
				OnShowObject();
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			if (Alarm.GkBaseEntity as GKDevice != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity as GKZone != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKZoneEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity as GKGuardZone != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKGuardZoneEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity as GKDirection != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDirectionEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity as GKMPT != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKMPTEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity as GKDelay != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDelayEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity as GKDoor != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDoorEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
		}
		void GetEvent<T>(Guid gkBaseEntityUid) where T : CompositePresentationEvent<Guid>, new()
		{
			ServiceFactory.Events.GetEvent<T>().Publish(gkBaseEntityUid);
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			GKDevice device;
			GKZone zone;
			GKGuardZone guardZone;
			GKDirection direction;
			GKMPT mpt;
			GKDelay delay;
			GKDoor door;

			if ((device = Alarm.GkBaseEntity as GKDevice) != null)
			{
				ShowOnPlanHelper.ShowDevice(device);
			}
			if ((zone = Alarm.GkBaseEntity as GKZone) != null)
			{
				ShowOnPlanHelper.ShowZone(zone);
			}
			if ((guardZone = Alarm.GkBaseEntity as GKGuardZone) != null)
			{
				ShowOnPlanHelper.ShowGuardZone(guardZone);
			}
			if ((direction = Alarm.GkBaseEntity as GKDirection) != null)
			{
				ShowOnPlanHelper.ShowDirection(direction);
			}
			if ((mpt = Alarm.GkBaseEntity as GKMPT) != null)
			{
				ShowOnPlanHelper.ShowMPT(mpt);
			}
			if ((delay = Alarm.GkBaseEntity as GKDelay) != null)
			{
				ShowOnPlanHelper.ShowDelay(delay);
			}
			if ((door = Alarm.GkBaseEntity as GKDoor) != null)
			{
				ShowOnPlanHelper.ShowDoor(door);
			}
		}
		bool CanShowOnPlan()
		{
			GKDevice device;
			GKZone zone;
			GKGuardZone guardZone;
			GKDirection direction;
			GKMPT mpt;
			GKDelay delay;
			GKDoor door;

			if ((device = Alarm.GkBaseEntity as GKDevice) != null)
			{
				return ShowOnPlanHelper.CanShowDevice(device);
			}
			if ((zone = Alarm.GkBaseEntity as GKZone) != null)
			{
				return ShowOnPlanHelper.CanShowZone(zone);
			}
			if ((guardZone = Alarm.GkBaseEntity as GKGuardZone) != null)
			{
				return ShowOnPlanHelper.CanShowGuardZone(guardZone);
			}
			if ((direction = Alarm.GkBaseEntity as GKDirection) != null)
			{
				return ShowOnPlanHelper.CanShowDirection(direction);
			}
			if ((mpt = Alarm.GkBaseEntity as GKMPT) != null)
			{
				return ShowOnPlanHelper.CanShowMPT(mpt);
			}
			if ((delay = Alarm.GkBaseEntity as GKDelay) != null)
			{
				return ShowOnPlanHelper.CanShowDelay(delay);
			}
			if ((door = Alarm.GkBaseEntity as GKDoor) != null)
			{
				return ShowOnPlanHelper.CanShowDoor(door);
			}
			return false;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var zone = Alarm.GkBaseEntity as GKZone;
				if (zone != null)
				{
					switch (Alarm.AlarmType)
					{
						case GKAlarmType.Fire1:
							ClientManager.FiresecService.GKResetFire1(zone);
							break;

						case GKAlarmType.Fire2:
							ClientManager.FiresecService.GKResetFire2(zone);
							break;
					}
				}
				var guardZone = Alarm.GkBaseEntity as GKGuardZone;
				if (guardZone != null)
				{
					switch (Alarm.AlarmType)
					{
						case GKAlarmType.GuardAlarm:
							ClientManager.FiresecService.GKReset(guardZone);
							break;
					}
				}
				var device = Alarm.GkBaseEntity as GKDevice;
				if (device != null)
				{
					ClientManager.FiresecService.GKReset(device);
				}
				var door = Alarm.GkBaseEntity as GKDoor;
				if (door != null)
				{
					switch (Alarm.AlarmType)
					{
						case GKAlarmType.GuardAlarm:
							ClientManager.FiresecService.GKReset(door);
							break;
					}
				}
			}
		}
		bool CanReset()
		{
			if (Alarm.GkBaseEntity as GKZone != null)
			{
				return (Alarm.AlarmType == GKAlarmType.Fire1 || Alarm.AlarmType == GKAlarmType.Fire2);
			}
			if (Alarm.GkBaseEntity as GKGuardZone != null)
			{
				return (Alarm.AlarmType == GKAlarmType.GuardAlarm);
			}

			var device = Alarm.GkBaseEntity as GKDevice;
			if (device != null)
			{
				if (device.DriverType == GKDriverType.RSR2_MAP4)
				{
					return device.State.StateClasses.Contains(XStateClass.Fire2) || device.State.StateClasses.Contains(XStateClass.Fire1);
				}
			}
			if (Alarm.GkBaseEntity as GKDoor != null)
			{
				return (Alarm.AlarmType == GKAlarmType.GuardAlarm);
			}
			return false;
		}
		public bool CanResetCommand
		{
			get { return CanReset(); }
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (Alarm.GkBaseEntity != null)
				{
					if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.GkBaseEntity);
					}
				}
			}
		}
		bool CanResetIgnore()
		{
			if (Alarm.AlarmType != GKAlarmType.Ignore)
				return false;
			if (Alarm.GkBaseEntity as GKDevice != null)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					return true;
			}

			if (Alarm.GkBaseEntity as GKZone != null)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
					return true;
			}

			if (Alarm.GkBaseEntity as GKGuardZone != null)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
					return true;
			}

			if (Alarm.GkBaseEntity as GKMPT != null)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
					return true;
			}

			if (Alarm.GkBaseEntity as GKDelay != null)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
					return true;
			}

			if (Alarm.GkBaseEntity as GKDirection != null)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
					return true;
			}
			return false;
		}
		public bool CanResetIgnoreCommand
		{
			get { return CanResetIgnore(); }
		}

		public RelayCommand TurnOnAutomaticCommand { get; private set; }
		void OnTurnOnAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var device = Alarm.GkBaseEntity as GKDevice;
				if (device != null)
				{
					if (device.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(device);
					}
				}

				var direction = Alarm.GkBaseEntity as GKDirection;
				if (direction != null)
				{
					if (direction.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(direction);
					}
				}

				var delay = Alarm.GkBaseEntity as GKDelay;
				if (delay != null)
				{
					if (delay.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(delay);
					}
				}

				var mpt = Alarm.GkBaseEntity as GKMPT;
				if (mpt != null)
				{
					if (mpt.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(mpt);
					}
				}
			}
		}
		bool CanTurnOnAutomatic()
		{
			if (Alarm.AlarmType == GKAlarmType.AutoOff)
			{
				if (Alarm.GkBaseEntity != null)
				{
					var device = Alarm.GkBaseEntity as GKDevice;
					if (device != null)
					{
						return device.Driver.IsControlDevice && Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.AutoOff);
					}
					return Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.AutoOff);
				}
			}
			return false;
		}
		public bool CanTurnOnAutomaticCommand
		{
			get { return CanTurnOnAutomatic(); }
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var uids = new List<Guid>();
			if (Alarm.GkBaseEntity != null)
				uids.Add(Alarm.GkBaseEntity.UID);
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(uids);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			GKDevice device;
			GKZone zone;
			GKGuardZone guardZone;
			GKDirection direction;
			GKMPT mpt;
			GKDelay delay;
			GKDoor door;

			if ((device = Alarm.GkBaseEntity as GKDevice) != null)
			{
				DialogService.ShowWindow(new DeviceDetailsViewModel(device));
			}
			if ((zone = Alarm.GkBaseEntity as GKZone) != null)
			{
				DialogService.ShowWindow(new ZoneDetailsViewModel(zone));
			}
			if ((guardZone = Alarm.GkBaseEntity as GKGuardZone) != null)
			{
				DialogService.ShowWindow(new GuardZoneDetailsViewModel(guardZone));
			}
			if ((direction = Alarm.GkBaseEntity as GKDirection) != null)
			{
				DialogService.ShowWindow(new DirectionDetailsViewModel(direction));
			}
			if ((mpt = Alarm.GkBaseEntity as GKMPT) != null)
			{
				DialogService.ShowWindow(new MPTDetailsViewModel(mpt));
			}
			if ((delay = Alarm.GkBaseEntity as GKDelay) != null)
			{
				DialogService.ShowWindow(new DelayDetailsViewModel(delay));
			}
			if ((door = Alarm.GkBaseEntity as GKDoor) != null)
			{
				DialogService.ShowWindow(new DoorDetailsViewModel(door));
			}
		}
		bool CanShowProperties()
		{
			return Alarm.GkBaseEntity != null;
		}
		public bool CanShowPropertiesCommand
		{
			get { return CanShowProperties(); }
		}
	}
}