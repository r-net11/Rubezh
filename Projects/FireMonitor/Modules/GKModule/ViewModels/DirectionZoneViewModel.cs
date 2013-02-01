using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DirectionZoneViewModel : ZoneViewModel
	{
		public DirectionZoneViewModel(XZoneState zoneState)
			: base(zoneState)
		{
		
		}
		public XStateType StateType { get; set; }
	}
}