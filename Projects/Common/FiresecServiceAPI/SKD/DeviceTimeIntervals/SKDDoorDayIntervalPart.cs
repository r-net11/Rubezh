using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDDoorDayIntervalPart
	{
		public SKDDoorDayIntervalPart()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public double StartMilliseconds { get; set; }

		[DataMember]
		public double EndMilliseconds { get; set; }

		[DataMember]
		public SKDDoorConfiguration_DoorOpenMethod DoorOpenMethod { get; set; }
	}
}