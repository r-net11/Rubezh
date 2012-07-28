using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace ItvIntegration
{
	public class ZoneViewModel : BaseViewModel
	{
		public ZoneViewModel(ZoneState zoneState)
		{
			ZoneState = zoneState;
			_stateType = zoneState.StateType;
			FiresecCallbackService.ZoneStateChangedEvent += new Action<int>(OnZoneStateChangedEvent);
			var zone = ItvManager.DeviceConfiguration.Zones.FirstOrDefault(x=>x.No == zoneState.No);
			if (zone != null)
			{
				Name = zone.PresentationName;
			}
		}

		void OnZoneStateChangedEvent(int zoneNo)
		{
			if (ZoneState.No == zoneNo)
			{
				StateType = ZoneState.StateType;
			}
		}

		public ZoneState ZoneState { get; private set; }

		public string Name { get; private set; }

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
	}
}