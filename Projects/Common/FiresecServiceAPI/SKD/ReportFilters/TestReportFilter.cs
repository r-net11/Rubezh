using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class TestReportFilter
	{
		[DataMember]
		public DateTime Timestamp { get; set; }
	}
}
