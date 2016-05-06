using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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
			StartDate = new DateTime(1900, 1, 1);
			EndDate = new DateTime(9000, 1, 1);
		}
	}
}