using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class NowStep : ProcedureStep
	{
		public NowStep()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Now; } }
	}
}
