using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Enums;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(DepartmentsReportFilter))]
	public class DepartmentsReportFilter : SKDReportFilter, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterArchive
	{
		public DepartmentsReportFilter()
		{
			ReportType = Enums.ReportType.DepartmentsReport;
		}

		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion IReportFilterOrganisation Members

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion IReportFilterDepartment Members

		#region IReportFilterArchive Members

		[DataMember]
		public bool UseArchive { get; set; }

		#endregion IReportFilterArchive Members
	}
}