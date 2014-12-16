using System;
using System.Collections.Generic;
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
			HolderUIDs = new List<Guid>();
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

		[DataMember]
		public List<Guid> HolderUIDs { get; set; }
	}

	public class CardReportItem
	{
		public CardReportItem()
		{
			UID = Guid.NewGuid();
		}

		public Guid UID { get; set; }
		public string CardType { get; set; }
		public int Number { get; set; }
		public string Organisation { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }
		public string Employee { get; set; }
		public string EndDate { get; set; }
	}
}