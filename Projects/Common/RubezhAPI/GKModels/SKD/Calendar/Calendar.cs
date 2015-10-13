using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	[DataContract]
	public class Calendar
	{
		public Calendar()
		{
			Year = DateTime.Now.Year;
			SelectedDays = new List<DateTime>();
		}

		[DataMember]
		public int Year { get; set; }

		[DataMember]
		public List<DateTime> SelectedDays { get; set; }
	}
}