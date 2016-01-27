﻿using FiresecAPI.GK;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoorDayInterval : ModelBase
	{
		public SKDDoorDayInterval()
		{
			DayIntervalParts = new List<SKDDoorDayIntervalPart>();
		}

		[DataMember]
		public List<SKDDoorDayIntervalPart> DayIntervalParts { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is SKDDoorDayInterval)
				return ((SKDDoorDayInterval)obj).UID == UID;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return No.GetHashCode();
		}
	}
}