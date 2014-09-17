using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ProcedureSelectionArguments
	{
		public ProcedureSelectionArguments()
		{
			ScheduleProcedure = new ScheduleProcedure();
		}

		[DataMember]
		public ScheduleProcedure ScheduleProcedure { get; set; }
	}
}
