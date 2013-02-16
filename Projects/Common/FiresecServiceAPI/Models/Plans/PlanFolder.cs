using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class PlanFolder
	{
		public PlanFolder()
		{
			Caption = "Папка";
			Children = new List<PlanFolder>();
		}

		public PlanFolder Parent { get; set; }

		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<PlanFolder> Children { get; set; }
	}
}
