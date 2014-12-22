using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class TestReportFilter : SKDReportFilter, IReportFilterPeriod
	{
		[DataMember]
		public DateTime Timestamp { get; set; }

		#region IReportFilterPeriod Members

		public ReportPeriodType PeriodType { get; set; }
		public DateTime DateTimeFrom { get; set; }
		public DateTime DateTimeTo { get; set; }

		#endregion
	}
}
