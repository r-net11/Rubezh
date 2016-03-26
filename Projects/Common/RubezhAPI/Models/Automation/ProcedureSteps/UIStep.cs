using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public abstract class UIStep : ProcedureStep
	{
		public UIStep()
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
