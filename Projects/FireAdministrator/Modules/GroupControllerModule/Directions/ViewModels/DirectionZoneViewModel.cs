using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class DirectionZoneViewModel
	{
		public DirectionZoneViewModel(GKDirectionZone directionZone)
		{
			DirectionZone = directionZone;
		}

		public GKDirectionZone DirectionZone { get; private set; }

		public GKStateBit StateType
		{
			get { return DirectionZone.StateBit; }
			set
			{
				DirectionZone.StateBit = value;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public List<GKStateBit> StateTypes
		{
			get
			{
				var stateTypes = new List<GKStateBit>();
				stateTypes.Add(GKStateBit.Attention);
				stateTypes.Add(GKStateBit.Fire1);
				stateTypes.Add(GKStateBit.Fire2);
				return stateTypes;
			}
		}
	}
}