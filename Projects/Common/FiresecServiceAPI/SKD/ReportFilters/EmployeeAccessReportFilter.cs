using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Enums;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(EmployeeAccessReportFilter))]
	public class EmployeeAccessReportFilter : SKDReportFilter, IReportFilterPassCardType, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor, IReportFilterZone, IReportFilterEmployee
	{
		public EmployeeAccessReportFilter()
		{
			PassCardActive = true;
			PassCardForcing = true;
			PassCardLocked = true;
			PassCardGuest = true;
			PassCardPermanent = true;
			PassCardTemprorary = true;
			IsEmployee = true;
			ReportType = ReportType.EmployeeAccessReport;
		}

		#region IReportFilterPassCardType Members

		[DataMember]
		public bool PassCardActive { get; set; }

		[DataMember]
		public bool PassCardPermanent { get; set; }

		[DataMember]
		public bool PassCardTemprorary { get; set; }

		[DataMember]
		public bool PassCardGuest { get; set; }

		[DataMember]
		public bool PassCardForcing { get; set; }

		[DataMember]
		public bool PassCardLocked { get; set; }

		#endregion IReportFilterPassCardType Members

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
	}
}