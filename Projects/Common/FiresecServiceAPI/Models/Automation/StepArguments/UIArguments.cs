using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class UIArguments
	{
		public UIArguments()
		{
			LayoutFilter = new ProcedureLayoutCollection();
			ForAllClients = true;
		}

		[DataMember]
		public ProcedureLayoutCollection LayoutFilter { get; set; }

		[DataMember]
		public bool ForAllClients { get; set; }
	}
}
