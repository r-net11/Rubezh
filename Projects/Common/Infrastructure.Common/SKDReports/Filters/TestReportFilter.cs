using System;
using System.Runtime.Serialization;

namespace Infrastructure.Common.SKDReports.Filters
{
	[DataContract]
	public class TestReportFilter
	{
		[DataMember]
		public DateTime Timestamp { get; set; }
	}
}
