using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecAPI.XModels.Automation
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
