using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class DayIntervalPartFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> DayIntervalUIDs { get; set; }
	}
}