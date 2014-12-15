using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
