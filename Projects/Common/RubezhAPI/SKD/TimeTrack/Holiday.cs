using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class Holiday : OrganisationElementBase, IOrganisationElement, IHRListItem
	{
		public Holiday()
		{
			Type = HolidayType.Holiday;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public HolidayType Type { get; set; }

		[DataMember]
		public DateTime Date { get; set; }

		[DataMember]
		public DateTime? TransferDate { get; set; }

		[DataMember]
		public TimeSpan Reduction { get; set; }

		[DataMember]
		public string Description { get; set; }

		public string ImageSource { get { return "/Controls;component/Images/Holiday.png"; } }
	}
}