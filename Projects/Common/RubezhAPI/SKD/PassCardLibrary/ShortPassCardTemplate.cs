using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ShortPassCardTemplate : IOrganisationElement, IHRListItem
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public bool IsDeleted { get; set; }

		[DataMember]
		public DateTime RemovalDate { get; set; }

		public string ImageSource { get { return "/Controls;component/Images/BPassCardDesigner.png"; } }
	}
}