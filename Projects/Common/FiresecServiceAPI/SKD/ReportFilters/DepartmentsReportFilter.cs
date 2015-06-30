using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class DepartmentsReportFilter : SKDReportFilter, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterArchive
	{
		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion

		#region IReportFilterArchive Members

		[DataMember]
		public bool UseArchive { get; set; }

		#endregion
	}
}
