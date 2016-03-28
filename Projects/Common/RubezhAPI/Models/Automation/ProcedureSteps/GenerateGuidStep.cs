using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class GenerateGuidStep : ProcedureStep
	{
		public GenerateGuidStep()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.GenerateGuid; } }
	}
}
