using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
				if (Alarm.GkBaseEntity != null)
					return Alarm.GkBaseEntity.ImageSource;
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
				var gkBaseEntity = Alarm.GkBaseEntity as IPlanPresentable;
				if (gkBaseEntity != null)
				{
					foreach (var planElementUID in gkBaseEntity.PlanElementUIDs)
					{
						elementBase = elementUnion.FirstOrDefault(x => x.UID == planElementUID);
						if (elementBase != null)
						{
							var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
							alarmPlanViewModel.GkBaseEntity = Alarm.GkBaseEntity;
							Plans.Add(alarmPlanViewModel);
							break;
						}
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
			if (Alarm.GkBaseEntity is GKDevice)
			{
				ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity is GKZone)
			{
				ServiceFactory.Events.GetEvent<ShowGKZoneEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity is GKGuardZone)
			{
				ServiceFactory.Events.GetEvent<ShowGKGuardZoneEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity is GKDirection)
			{
				ServiceFactory.Events.GetEvent<ShowGKDirectionEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity is GKPumpStation)
			{
				ServiceFactory.Events.GetEvent<ShowGKPumpStationEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity is GKMPT)
			{
				ServiceFactory.Events.GetEvent<ShowGKMPTEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity is GKDelay)
			{
				ServiceFactory.Events.GetEvent<ShowGKDelayEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
			if (Alarm.GkBaseEntity is GKDoor)
			{
				ServiceFactory.Events.GetEvent<ShowGKDoorEvent>().Publish(Alarm.GkBaseEntity.UID);
			}
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
			//Camera camera;

			if ((device = Alarm.GkBaseEntity as GKDevice) != null)
			{
				ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(device.UID), device.UID);
			}
			if ((zone = Alarm.GkBaseEntity as GKZone) != null)
			{
				ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(zone.UID), zone.UID);
			}
			if ((guardZone = Alarm.GkBaseEntity as GKGuardZone) != null)
			{
				ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(guardZone.UID), guardZone.UID);
			}
			if ((direction = Alarm.GkBaseEntity as GKDirection) != null)
			{
				ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(direction.UID), direction.UID);
			}
			if ((mpt = Alarm.GkBaseEntity as GKMPT) != null)
			{
				ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(mpt.UID), mpt.UID);
			}
			if ((delay = Alarm.GkBaseEntity as GKDelay) != null)
			{
				ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(delay.UID), delay.UID);
			}
			if ((door = Alarm.GkBaseEntity as GKDoor) != null)
			{
				ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(door.UID), door.UID);
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
			//Camera camera;

			if ((device = Alarm.GkBaseEntity as GKDevice) != null)
			{
				return ShowOnPlanHelper.GetPlan(device.UID)!= null;
			}
			if ((zone = Alarm.GkBaseEntity as GKZone) != null)
			{
				return ShowOnPlanHelper.GetPlan(zone.UID)!= null;
			}
			if ((guardZone = Alarm.GkBaseEntity as GKGuardZone) != null)
			{
				return ShowOnPlanHelper.GetPlan(guardZone.UID)!= null;
			}
			if ((direction = Alarm.GkBaseEntity as GKDirection) != null)
			{
				return ShowOnPlanHelper.GetPlan(direction.UID)!= null;
			}
			if ((mpt = Alarm.GkBaseEntity as GKMPT) != null)
			{
				return ShowOnPlanHelper.GetPlan(mpt.UID)!= null;
			}
			if ((delay = Alarm.GkBaseEntity as GKDelay) != null)
			{
				return ShowOnPlanHelper.GetPlan(delay.UID)!= null;
			}
			if ((door = Alarm.GkBaseEntity as GKDoor) != null)
			{
				return ShowOnPlanHelper.GetPlan(door.UID)!= null;
			}
			//if ((camera = Alarm.GkBaseEntity as Camera) != null)
			//{
			//	return ShowOnPlanHelper.GetPlan(door.UID) != null;
			//}
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
			if (Alarm.GkBaseEntity is GKDevice)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKZone)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKGuardZone)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKMPT)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDelay)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDirection)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKPumpStation)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_NS_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDoor)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Door_Control))
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

				var pumpStation = Alarm.GkBaseEntity as GKPumpStation;
				if (pumpStation != null)
				{
					if (pumpStation.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_NS_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(pumpStation);
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

				var guardZone = Alarm.GkBaseEntity as GKGuardZone;
				if (guardZone != null)
				{
					if (guardZone.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(guardZone);
					}
				}

				var door = Alarm.GkBaseEntity as GKDoor;
				if (door != null)
				{
					if (door.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Door_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(door);
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
			GKPumpStation pumpStation;
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
			if ((pumpStation = Alarm.GkBaseEntity as GKPumpStation) != null)
			{
				DialogService.ShowWindow(new PumpStationDetailsViewModel(pumpStation));
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