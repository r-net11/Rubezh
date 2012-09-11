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
		public List<DeviceState> DeviceStates;
		public List<ZoneState> ZoneStates;
		StateType _selfState = StateType.No;

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
				if (elementRectangleZone.ZoneNo.HasValue)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == elementRectangleZone.ZoneNo.Value);
					if (zone != null)
						ZoneStates.Add(zone.ZoneState);
				}
			}
			foreach (var elementPolygonZone in plan.ElementPolygonZones)
			{
				if (elementPolygonZone.ZoneNo.HasValue)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == elementPolygonZone.ZoneNo.Value);
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
			_selfState = StateType.No;

			foreach (var zoneState in ZoneStates)
			{
				if (zoneState.StateType < _selfState)
					_selfState = zoneState.StateType;
			}

			UpdateState();
		}

		public void UpdateState()
		{
			StateType = _selfState;

			foreach (var planViewModel in Children)
			{
				if (planViewModel.StateType < StateType)
					StateType = planViewModel.StateType;
			}

			if (Parent != null)
				Parent.UpdateState();
		}
	}
}