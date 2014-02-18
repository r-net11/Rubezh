﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class DateTimePeriod
	{
		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		public DateTimePeriod()
		{
			StartDate = DateTime.Now.AddYears(-20);
			EndDate = DateTime.Now;
		}
	}
}