using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ScheduleProcedure
	{
		public ScheduleProcedure(Guid procedureUid)
		{
			Arguments = new List<Variable>();
			Uid = Guid.NewGuid();
			ProcedureUid = procedureUid;
		}

		[DataMember]
		public List<Variable> Arguments { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid ProcedureUid { get; set; }

	}
}
