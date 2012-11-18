using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using XFiresecAPI;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeBaseViewModel<PlanViewModel>
	{
		public Plan Plan { get; private set; }
        public List<DeviceState> DeviceStates { get; private set; }
		public List<ZoneState> ZoneStates { get; private set; }
		public List<XDeviceState> XDeviceStates { get; private set; }
		public List<XZoneState> XZoneStates { get; private set; }
		StateType SelfState = StateType.No;

		public PlanViewModel(Plan plan, ObservableCollection<PlanViewModel> source)
		{
			Plan = plan;
			Source = source;

			DeviceStates = new List<DeviceState>();
			foreach (var elementDevice in plan.ElementDevices)
			{
				var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
				if (device != null)
				{
					DeviceStates.Add(device.DeviceState);
					device.DeviceState.StateChanged += new Action(UpdateSelfState);
				}
			}
			ZoneStates = new List<ZoneState>();
			foreach (var elementRectangleZone in plan.ElementRectangleZones)
			{
				if (elementRectangleZone.ZoneUID != Guid.Empty)
				{
                    var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == elementRectangleZone.ZoneUID);
					if (zone != null)
						ZoneStates.Add(zone.ZoneState);
				}
			}
			foreach (var elementPolygonZone in plan.ElementPolygonZones)
			{
                if (elementPolygonZone.ZoneUID != Guid.Empty)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == elementPolygonZone.ZoneUID);
					if (zone != null)
						ZoneStates.Add(zone.ZoneState);
				}
			}
			XDeviceStates = new List<XDeviceState>();
			foreach (var elementXDevice in plan.ElementXDevices)
			{
				var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementXDevice.XDeviceUID);
				if (device != null)
				{
					XDeviceStates.Add(device.DeviceState);
					device.DeviceState.StateChanged += new Action(UpdateSelfState);
				}
			}
			XZoneStates = new List<XZoneState>();
			foreach (var elementRectangleXZone in plan.ElementRectangleXZones)
			{
				if (elementRectangleXZone.ZoneUID != Guid.Empty)
				{
					var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == elementRectangleXZone.ZoneUID);
					if (zone != null)
						XZoneStates.Add(zone.ZoneState);
				}
			}
			foreach (var elementPolygonXZone in plan.ElementPolygonXZones)
			{
				if (elementPolygonXZone.ZoneUID != Guid.Empty)
				{
					var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == elementPolygonXZone.ZoneUID);
					if (zone != null)
						XZoneStates.Add(zone.ZoneState);
				}
			}
			UpdateSelfState();
		}

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(Plan.UID);
				OnPropertyChanged("StateType");
			}
		}

		public void UpdateSelfState()
		{
			SelfState = StateType.No;
            foreach (var state in DeviceStates)
			{
				if (state.StateType < SelfState)
					SelfState = state.StateType;
			}
            foreach (var state in ZoneStates)
            {
                if (state.StateType < SelfState)
                    SelfState = state.StateType;
            }
			foreach (var state in XDeviceStates)
			{
                if (state.GetStateType() < SelfState)
                    SelfState = state.GetStateType();
			}
			foreach (var state in XZoneStates)
			{
                if (state.GetStateType() < SelfState)
                    SelfState = state.GetStateType();
			}
			UpdateState();
		}

		public void UpdateState()
		{
			StateType = SelfState;
			foreach (var child in Children)
			{
				if (child.StateType < StateType)
					StateType = child.StateType;
			}
			if (Parent != null)
				Parent.UpdateState();
		}
	}
}