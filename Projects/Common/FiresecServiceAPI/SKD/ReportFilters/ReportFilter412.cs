using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class ReportFilter412 : SKDReportFilter, IReportFilterPassCardType, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployee, IReportFilterZone
	{
		#region IReportFilterPassCardType Members

		[DataMember]
		public bool PassCardActive { get; set; }
		[DataMember]
		public bool PassCardPermanent { get; set; }
		[DataMember]
		public bool PassCardTemprorary { get; set; }
		[DataMember]
		public bool PassCardOnceOnly { get; set; }
		[DataMember]
		public bool PassCardForcing { get; set; }
		[DataMember]
		public bool PassCardLocked { get; set; }

		#endregion

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

		#endregion

		#region IReportFilterZone Members

		[DataMember]
		public List<Guid> Zones {get;set;}

		#endregion
	}
}
