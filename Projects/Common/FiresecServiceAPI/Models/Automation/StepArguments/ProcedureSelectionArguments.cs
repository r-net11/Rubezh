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
			Uid = Guid.NewGuid();
			ScheduleProcedure = new ScheduleProcedure();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ScheduleProcedure ScheduleProcedure { get; set; }
	}
}
