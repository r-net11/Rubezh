using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class PlanFolder
	{
		public PlanFolder()
		{
			Caption = "Папка";
			Plans = new List<Plan>();
			Folders = new List<PlanFolder>();
		}

		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<Plan> Plans { get; set; }
		[DataMember]
		public List<PlanFolder> Folders { get; set; }
	}
}
