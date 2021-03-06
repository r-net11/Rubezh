﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD.ReportFilters
{
	[DataContract]
	public class EmployeeReportFilter : SKDReportFilter, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor, IReportFilterArchive
	{
		public EmployeeReportFilter()
		{
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

		#region IReportFilterArchive Members

		[DataMember]
		public bool UseArchive { get; set; }

		#endregion
	}
}
