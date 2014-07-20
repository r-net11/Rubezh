using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	[KnownType(typeof(LayoutPartImageProperties))]
	[KnownType(typeof(LayoutPartCameraProperties))]
	[KnownType(typeof(LayoutPartJournalProperties))]
	[KnownType(typeof(LayoutPartSKDVerificationProperties))]
	[KnownType(typeof(LayoutPartPlansProperties))]
	public class LayoutPart
	{
		public LayoutPart()
		{
		}

		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public Guid DescriptionUID { get; set; }
		[DataMember]
		public ILayoutProperties Properties { get; set; }
	}
}