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

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeBaseViewModel<PlanViewModel>
	{
		public Plan Plan { get; private set; }
        public List<DeviceState> DeviceStates { get; private set; }
		List<ZoneState> ZoneStates;
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
            foreach (var deviceState in DeviceStates)
			{
				if (deviceState.StateType < SelfState)
					SelfState = deviceState.StateType;
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