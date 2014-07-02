using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class ProcedureSelectionArguments
	{
		public ProcedureSelectionArguments()
		{
			Uid = Guid.NewGuid();
			ScheduleProcedures = new List<ScheduleProcedure>();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public List<ScheduleProcedure> ScheduleProcedures { get; set; }
	}
}
