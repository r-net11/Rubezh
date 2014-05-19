using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class DirectionZoneViewModel
	{
		public DirectionZoneViewModel(XDirectionZone directionZone)
		{
			DirectionZone = directionZone;
		}

		public XDirectionZone DirectionZone { get; private set; }

		public XStateBit StateType
		{
			get { return DirectionZone.StateBit; }
			set
			{
				DirectionZone.StateBit = value;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public List<XStateBit> StateTypes
		{
			get
			{
				var stateTypes = new List<XStateBit>();
				stateTypes.Add(XStateBit.Attention);
				stateTypes.Add(XStateBit.Fire1);
				stateTypes.Add(XStateBit.Fire2);
				return stateTypes;
			}
		}
	}
}