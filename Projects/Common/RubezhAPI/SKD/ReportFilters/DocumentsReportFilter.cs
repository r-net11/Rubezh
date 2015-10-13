using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD.ReportFilters
{
	[DataContract]
	public class DocumentsReportFilter : SKDReportFilter, IReportFilterPeriod, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterEmployee
	{
		public DocumentsReportFilter()
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
		[DataMember]
		public bool IsSearch { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string SecondName { get; set; }

		#endregion

		[DataMember]
		public bool Overtime { get; set; }
		[DataMember]
		public bool Presence { get; set; }
		[DataMember]
		public bool Abcense { get; set; }
	}
}
