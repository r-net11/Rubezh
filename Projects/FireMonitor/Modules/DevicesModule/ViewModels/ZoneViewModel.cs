﻿using System;
using System.Linq;
using DevicesModule.Events;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System.Collections.Generic;

namespace DevicesModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public ZoneState ZoneState { get; private set; }
		public Zone Zone
		{
			get { return ZoneState.Zone; }
		}
		List<Device> devices;
		List<DeviceState> deviceStates;

		public ZoneViewModel(ZoneState zoneState)
		{
			SelectCommand = new RelayCommand(OnSelect);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			DisableAllCommand = new RelayCommand(OnDisableAll, CanDisableAll);
			EnableAllCommand = new RelayCommand(OnEnableAll, CanEnableAll);
			SetGuardCommand = new RelayCommand(OnSetGuard, CanSetGuard);
			UnSetGuardCommand = new RelayCommand(OnUnSetGuard, CanUnSetGuard);

			ZoneState = zoneState;
			ZoneState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
			InitializeDevices();
		}

		void OnStateChanged()
		{
			StateType = ZoneState.StateType;
			OnPropertyChanged("ZoneState");
			OnPropertyChanged("Tooltip");
		}

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				OnPropertyChanged("StateType");
			}
		}

		public string Tooltip
		{
			get
			{
				var toolTip = Zone.PresentationName;
				toolTip += "\n" + "Состояние: " + StateType.ToDescription();
				if (Zone.ZoneType == ZoneType.Fire)
				{
					toolTip += "\n" + "Количество датчиков для сработки: " + Zone.DetectorCount.ToString();
				}
				if (Zone.ZoneType == ZoneType.Guard)
				{
					if (FiresecManager.IsZoneOnGuardAlarm(ZoneState))
						toolTip += "\n" + "Охранная тревога";
					else
					{
						if (FiresecManager.IsZoneOnGuard(ZoneState))
							toolTip += "\n" + "На охране";
						else
							toolTip += "\n" + "Не на охране";
					}
				}
				return toolTip;
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(Zone.UID);
		}

		bool CanShowOnPlan()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
                if (plan.ElementPolygonZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == Zone.UID)))
				{
					return true;
				}
                if (plan.ElementRectangleZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == Zone.UID)))
				{
					return true;
				}
			}
			return false;
		}

		void InitializeDevices()
		{
			devices = new List<Device>();
			deviceStates = new List<DeviceState>();
			foreach (var device in FiresecManager.Devices)
			{
				if ((device != null) && (device.Driver != null))
				{
					if ((device.ZoneUID == ZoneState.Zone.UID) && (device.Driver.CanDisable))
					{
						devices.Add(device);
						deviceStates.Add(device.DeviceState);
					}
				}
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
            ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Publish(Zone.UID);
		}

		public RelayCommand DisableAllCommand { get; private set; }
		void OnDisableAll()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.FiresecDriver.AddToIgnoreList(devices);
		}
		bool CanDisableAll()
		{
			if (Zone.ZoneType == ZoneType.Guard)
				return false;
			return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList) && deviceStates.Any(x => !x.IsDisabled));
		}

		public RelayCommand EnableAllCommand { get; private set; }
		void OnEnableAll()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.FiresecDriver.RemoveFromIgnoreList(devices);
		}
		bool CanEnableAll()
		{
			if (Zone.ZoneType == ZoneType.Guard)
				return false;
			return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_AddToIgnoreList) && deviceStates.Any(x => x.IsDisabled));
		}

		public RelayCommand SetGuardCommand { get; private set; }
		void OnSetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.SetZoneGuard(Zone);
		}
		bool CanSetGuard()
		{
			return ((Zone.ZoneType == ZoneType.Guard) && (Zone.SecPanelUID != null) && (FiresecManager.IsZoneOnGuard(ZoneState) == false));
		}

		public RelayCommand UnSetGuardCommand { get; private set; }
		void OnUnSetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.UnSetZoneGuard(Zone);
		}
		bool CanUnSetGuard()
		{
			return ((Zone.ZoneType == ZoneType.Guard) && (Zone.SecPanelUID != null) && (FiresecManager.IsZoneOnGuard(ZoneState) == true));
		}
	}
}