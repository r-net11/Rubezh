using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class PtzStep : ProcedureStep
	{
		public PtzStep()
		{
			CameraArgument = new Argument();
			PtzNumberArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument PtzNumberArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Ptz; } }
	}
}
