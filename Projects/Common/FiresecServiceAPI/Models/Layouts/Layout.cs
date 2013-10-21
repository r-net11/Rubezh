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
			Users = new List<Guid>();
			Parts = new List<Guid>();
			Caption = "Шаблон";
		}

		[DataMember]
		public List<Guid> Users { get; set; }
		[DataMember]
		public List<Guid> Parts { get; set; }
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
