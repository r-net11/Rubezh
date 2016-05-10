using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ScheduleDayIntervalFilter : IsDeletedFilter
	{
		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}