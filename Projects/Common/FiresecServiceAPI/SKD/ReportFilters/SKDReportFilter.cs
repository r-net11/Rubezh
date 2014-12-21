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

			PrintName = true;
			PrintNameInHeader = false;
			PrintPeriod = true;
			PrintDate = true;
			PrintUser = true;

			SortAscending = true;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string SortColumn { get; set; }
		[DataMember]
		public bool SortAscending { get; set; }

		[DataMember]
		public bool PrintName { get; set; }
		[DataMember]
		public bool PrintNameInHeader { get; set; }
		[DataMember]
		public bool PrintPeriod { get; set; }
		[DataMember]
		public bool PrintDate { get; set; }
		[DataMember]
		public bool PrintUser { get; set; }
	}
}
