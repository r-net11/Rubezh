using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class RunProgramStep : ProcedureStep
	{
		public RunProgramStep()
		{
			PathArgument = new Argument();
			ParametersArgument = new Argument();
		}

		[DataMember]
		public Argument PathArgument { get; set; }

		[DataMember]
		public Argument ParametersArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.RunProgram; } }
		public override Argument[] Arguments
		{
			get { return new Argument[] { PathArgument, ParametersArgument }; }
		}
	}
}