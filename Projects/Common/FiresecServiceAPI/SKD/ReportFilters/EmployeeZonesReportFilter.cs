using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Enums;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(EmployeeZonesReportFilter))]
	public class EmployeeZonesReportFilter : SKDReportFilter, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor, IReportFilterZone
	{
		public EmployeeZonesReportFilter()
		{
			UseCurrentDate = true;
			IsEmployee = true;
			ReportDateTime = DateTime.Now;
			ReportType = Enums.ReportType.EmployeeZonesReport;
		}

		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion IReportFilterOrganisation Members

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion IReportFilterDepartment Members

		#region IReportFilterPosition Members

		[DataMember]
		public List<Guid> Positions { get; set; }

		#endregion IReportFilterPosition Members

		#region IReportFilterEmployee Members

		[DataMember]
		public List<Guid> Employees { get; set; }

		[DataMember]
		public bool IsSearch { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		#endregion IReportFilterEmployee Members

		#region IReportFilterEmployeeAndVisitor Members

		[DataMember]
		public bool IsEmployee { get; set; }

		#endregion IReportFilterEmployeeAndVisitor Members

		#region IReportFilterZone Members

		[DataMember]
		public List<Guid> Zones { get; set; }

		#endregion IReportFilterZone Members

		[DataMember]
		public bool UseCurrentDate { get; set; }

		[DataMember]
		public DateTime? ReportDateTime { get; set; }
	}
}