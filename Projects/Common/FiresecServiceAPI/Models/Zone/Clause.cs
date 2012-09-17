using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Clause
	{
		public Clause()
		{
			ZoneNos = new List<int>();
			State = ZoneLogicState.Fire;
			Operation = ZoneLogicOperation.Any;
			Zones = new List<Zone>();
		}

		public Device Device { get; set; }
		public List<Zone> Zones { get; set; }

		[DataMember]
		public ZoneLogicState State { get; set; }

		[DataMember]
		public ZoneLogicOperation? Operation { get; set; }

		[DataMember]
		public List<int> ZoneNos { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		public bool CanSelectOperation
		{
			get
			{
				return (State == ZoneLogicState.Fire) ||
					(State == ZoneLogicState.Attention) ||
					(State == ZoneLogicState.MPTAutomaticOn) ||
					(State == ZoneLogicState.MPTOn) ||
					(State == ZoneLogicState.Alarm) ||
					(State == ZoneLogicState.GuardSet) ||
					(State == ZoneLogicState.GuardUnSet);
			}
		}
	}
}