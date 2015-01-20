using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
    public class ReportFilter423 : SKDReportFilter, IReportFilterPeriod, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterEmployee
	{
		public ReportFilter423()
		{
			Overtime = true;
			Presence = true;
			Abcense = true;
		}

		#region IReportFilterPeriod Members

		[DataMember]
		public ReportPeriodType PeriodType { get; set; }
		[DataMember]
		public DateTime DateTimeFrom { get; set; }
		[DataMember]
		public DateTime DateTimeTo { get; set; }

		#endregion

		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion

		#region IReportFilterEmployee Members

		[DataMember]
		public List<Guid> Employees { get; set; }

		#endregion

		[DataMember]
		public bool Overtime { get; set; }
		[DataMember]
		public bool Presence { get; set; }
		[DataMember]
		public bool Abcense { get; set; }
	}
}
