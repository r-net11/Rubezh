using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ProcedureStep
	{
		public ProcedureStep()
		{
			UID = Guid.NewGuid();
			Children = new List<ProcedureStep>();
		}

		public ProcedureStep Parent { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<ProcedureStep> Children { get; set; }

		[DataMember]
		public ProcedureStepType ProcedureStepType { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public SoundArguments SoundArguments { get; set; }
	}
}