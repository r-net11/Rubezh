using RubezhAPI.Models;
using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD.ReportFilters
{
	[DataContract]
	public class SKDReportFilter
	{
		public SKDReportFilter()
		{
			Name = DefaultFilterName;

			PrintFilterName = true;
			PrintFilterNameInHeader = false;
			PrintPeriod = true;
			PrintDate = true;
			PrintUser = true;

			SortAscending = true;

			if (this is IReportFilterPeriod)
			{
				var periodFilter = (IReportFilterPeriod)this;
				periodFilter.DateTimeFrom = DateTime.Today;
				periodFilter.DateTimeTo = DateTime.Today.AddDays(1).AddSeconds(-1);
				periodFilter.PeriodType = ReportPeriodType.Arbitrary;
			}
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public DateTime Timestamp { get; set; }
		[DataMember]
		public User User { get; set; }

		[DataMember]
		public string SortColumn { get; set; }
		[DataMember]
		public bool SortAscending { get; set; }

		[DataMember]
		public bool PrintFilterName { get; set; }
		[DataMember]
		public bool PrintFilterNameInHeader { get; set; }
		[DataMember]
		public bool PrintPeriod { get; set; }
		[DataMember]
		public bool PrintDate { get; set; }
		[DataMember]
		public bool PrintUser { get; set; }

		public bool IsDefault { get { return Name == DefaultFilterName; } }
		string DefaultFilterName { get { return "По умолчанию"; } }
	}
}