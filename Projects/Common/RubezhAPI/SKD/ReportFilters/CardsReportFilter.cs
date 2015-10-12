using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD.ReportFilters
{
	[DataContract]
	public class CardsReportFilter : SKDReportFilter, IReportFilterPassCardTypeFull, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor, IReportFilterEmployee
	{
		public CardsReportFilter()
		{
			ExpirationDate = DateTime.Today;
			ExpirationType = EndDateType.Day;
			PassCardActive = true;
			PassCardForcing = true;
			PassCardInactive = true;
			PassCardLocked = true;
			PassCardOnceOnly = true;
			PassCardPermanent = true;
			PassCardTemprorary = true;
			IsEmployee = true;
		}

		#region IReportFilterPassCardTypeFull Members

		[DataMember]
		public bool PassCardInactive { get; set; }

		#endregion

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

		[DataMember]
		public bool UseExpirationDate { get; set; }
		[DataMember]
		public DateTime ExpirationDate { get; set; }
		[DataMember]
		public EndDateType ExpirationType { get; set; }
	}
}
