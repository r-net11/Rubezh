using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class SKDReportFilter
	{
		public SKDReportFilter()
		{
			Name = "По умолчанию";

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
                periodFilter.PeriodType = ReportPeriodType.Day;
            }
		}

		[DataMember]
		public string Name { get; set; }
        [DataMember]
        public string User { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }

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
	}
}
