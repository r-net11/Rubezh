using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutFolder
	{
		public LayoutFolder()
		{
			Folders = new List<LayoutFolder>();
			Layouts = new List<Layout>();
			Caption = "Папка";
		}

		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public List<LayoutFolder> Folders { get; set; }
		[DataMember]
		public List<Layout> Layouts { get; set; }
	}
}
