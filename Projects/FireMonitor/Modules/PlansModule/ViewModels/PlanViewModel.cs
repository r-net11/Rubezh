using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using FiresecAPI;

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
				var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
				if (deviceState != null)
				{
					DeviceStates.Add(deviceState);
					deviceState.StateChanged += new Action(UpdateSelfState);
				}
			}
			ZoneStates = new List<ZoneState>();
			foreach (var elementRectangleZone in plan.ElementRectangleZones)
			{
				if (elementRectangleZone.ZoneNo.HasValue)
				{
					var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == elementRectangleZone.ZoneNo.Value);
					if (zoneState != null)
						ZoneStates.Add(zoneState);
				}
			}
			foreach (var elementPolygonZone in plan.ElementPolygonZones)
			{
				if (elementPolygonZone.ZoneNo.HasValue)
				{
					var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == elementPolygonZone.ZoneNo.Value);
					if (zoneState != null)
						ZoneStates.Add(zoneState);
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