using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class Calendar
	{
		public Calendar()
		{
			Year = DateTime.Now.Year;
		}

		[DataMember]
		public int Year { get; set; }

		[DataMember]
		public List<DateTime> SelectedDays { get; set; }
	}
}
