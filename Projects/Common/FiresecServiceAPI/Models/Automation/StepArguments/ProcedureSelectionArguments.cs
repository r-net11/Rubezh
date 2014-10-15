using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract, Serializable]
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