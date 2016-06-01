using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(CardsReportFilter))]
	public class CardsReportFilter : SKDReportFilter, IReportFilterPassCardTypeFull, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor
	{
		public CardsReportFilter()
		{
			ExpirationDate = DateTime.Today;
			ExpirationType = EndDateType.Day;
			PassCardActive = true;
			PassCardForcing = true;
			PassCardInactive = true;
			PassCardLocked = true;
			PassCardGuest = true;
			PassCardPermanent = true;
			PassCardTemprorary = true;
			IsEmployee = true;
			ReportType = Enums.ReportType.CardsReport;
		}

		#region IReportFilterPassCardTypeFull Members

		[DataMember]
		public bool PassCardInactive { get; set; }

		#endregion IReportFilterPassCardTypeFull Members

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

		[DataMember]
		public bool UseExpirationDate { get; set; }

		[DataMember]
		public DateTime ExpirationDate { get; set; }

		[DataMember]
		public EndDateType ExpirationType { get; set; }
	}
}