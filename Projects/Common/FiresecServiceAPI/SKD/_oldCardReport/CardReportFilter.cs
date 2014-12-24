using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
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
}
