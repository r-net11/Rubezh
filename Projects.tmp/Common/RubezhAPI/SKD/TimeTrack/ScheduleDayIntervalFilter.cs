using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ScheduleDayIntervalFilter : IsDeletedFilter
	{
		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}