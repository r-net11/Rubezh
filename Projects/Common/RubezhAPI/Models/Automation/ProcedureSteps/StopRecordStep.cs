using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class StopRecordStep : ProcedureStep
	{
		public StopRecordStep()
		{
			CameraArgument = new Argument();
			EventUIDArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument EventUIDArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.StopRecord; } }
		public override Argument[] Arguments
		{
			get { return new Argument[] { CameraArgument, EventUIDArgument }; }
		}
	}
}