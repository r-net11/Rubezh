using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ProcedureLayoutCollection
	{
		public ProcedureLayoutCollection()
		{
			LayoutsUIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> LayoutsUIDs { get; set; }
	}
}