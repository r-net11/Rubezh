using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class Layout
	{
		public Layout()
		{
			UID = Guid.NewGuid();
			Users = new List<Guid>();
			Parts = new List<LayoutPart>();
			Caption = "Шаблон";
		}

		[DataMember]
		public List<Guid> Users { get; set; }
		[DataMember]
		public List<LayoutPart> Parts { get; set; }
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public string Content { get; set; }
	}
}
