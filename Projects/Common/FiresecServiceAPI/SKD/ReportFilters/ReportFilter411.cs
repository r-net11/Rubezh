using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class ReportFilter411 : SKDReportFilter, IReportFilterPassCardTypeFull, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployee
	{
		public ReportFilter411()
		{
			ExpirationDate = DateTime.Today;
			ExpirationType = EndDateType.Day;
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

		#endregion

		#region IReportFilterPassCardType Members

		[DataMember]
		public bool PassCardActive { get; set; }
		[DataMember]
		public bool PassCardInactive { get; set; }
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

		[DataMember]
		public bool UseExpirationDate { get; set; }
		[DataMember]
		public DateTime ExpirationDate { get; set; }
		[DataMember]
		public EndDateType ExpirationType { get; set; }
	}
}
