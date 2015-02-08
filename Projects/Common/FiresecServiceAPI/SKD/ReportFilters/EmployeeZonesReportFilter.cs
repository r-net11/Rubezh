using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class EmployeeZonesReportFilter : SKDReportFilter, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor, IReportFilterZone
	{
		public EmployeeZonesReportFilter()
		{
			ReportDateTime = DateTime.Now;
			UseCurrentDate = true;
			IsEmployee = true;
		}

		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion

		#region IReportFilterPosition Members

		[DataMember]
		public List<Guid> Positions { get; set; }

		#endregion

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

		#endregion

		#region IReportFilterEmployeeAndVisitor Members

		[DataMember]
		public bool IsEmployee { get; set; }

		#endregion

		#region IReportFilterZone Members

		[DataMember]
		public List<Guid> Zones { get; set; }

		#endregion

		[DataMember]
		public bool UseCurrentDate { get; set; }
		[DataMember]
		public DateTime ReportDateTime { get; set; }
	}
}
