using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardFilter : OrganisationFilterBase
	{
		public CardFilter()
			: base()
		{
			DeactivationType = LogicalDeletationType.All;
			CardTypes = new List<CardType>();
			EndDate = DateTime.Now.Date;
		}

		[DataMember]
		public LogicalDeletationType DeactivationType { get; set; }

		[DataMember]
		public bool IsWithEndDate { get; set; }
		
		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public List<CardType> CardTypes { get; set; }
	}

	[DataContract]
	public class CardReportFilter
	{
		public CardReportFilter()
		{
			UID = Guid.NewGuid();
			CardFilter = new CardFilter();
			EmployeeFilter = new EmployeeFilter();
		}

		[DataMember]
		public Guid UID { get; set; }
		
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public CardFilter CardFilter { get; set; }

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		[DataMember]
		public CardSortType CardSortType { get; set; }

		[DataMember]
		public bool IsSortAsc { get; set; }

		[DataMember]
		public bool IsShowUser { get; set; }

		[DataMember]
		public bool IsShowDate { get; set; }

		[DataMember]
		public bool IsShowPeriod { get; set; }

		[DataMember]
		public bool IsShowNameInHeader { get; set; }

		[DataMember]
		public bool IsShowName { get; set; }
	}

	public enum CardSortType
	{
		[Description("Статус")]
		Status,
		
		[Description("Номер")]
		Number,
		
		[Description("Организация")]
		Organisation,
		
		[Description("Подразделение")]
		Department,
		
		[Description("Должность")]
		Position,
		
		[Description("Сотрудник")]
		Employee,
		
		[Description("Срок действия")]
		Duration
	}
}