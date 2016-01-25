using System.Runtime.Serialization;

namespace RubezhAPI.Automation
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