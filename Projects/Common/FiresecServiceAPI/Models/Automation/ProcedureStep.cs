using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ProcedureStep
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ProcedureStepType ProcedureStepType { get; set; }
	}
}