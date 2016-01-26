using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class UIArguments
	{
		public UIArguments()
		{
			LayoutFilter = new List<Guid>();
			ForAllClients = true;
		}

		[DataMember]
		public List<Guid> LayoutFilter { get; set; }

		[DataMember]
		public bool ForAllClients { get; set; }
	}
}
