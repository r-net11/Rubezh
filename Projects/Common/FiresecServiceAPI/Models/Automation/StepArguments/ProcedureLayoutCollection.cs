using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
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