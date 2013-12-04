using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	[KnownType(typeof(LayoutPartImageProperties))]
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