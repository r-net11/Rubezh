using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class GetListCountStep : ProcedureStep
	{
		public GetListCountStep()
		{
			ListArgument = new Argument();
			CountArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument CountArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.GetListCount; } }
	}
}