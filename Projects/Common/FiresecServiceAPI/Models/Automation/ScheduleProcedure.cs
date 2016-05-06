using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ScheduleProcedure
	{
		public ScheduleProcedure()
		{
			Arguments = new List<Argument>();
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public List<Argument> Arguments { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid ProcedureUid { get; set; }
	}
}