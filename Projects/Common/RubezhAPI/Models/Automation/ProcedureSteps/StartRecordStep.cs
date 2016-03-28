using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class StartRecordStep : ProcedureStep
	{
		public StartRecordStep()
		{
			CameraArgument = new Argument();
			EventUIDArgument = new Argument();
			TimeoutArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument EventUIDArgument { get; set; }

		[DataMember]
		public Argument TimeoutArgument { get; set; }

		[DataMember]
		public TimeType TimeType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.StartRecord; } }
	}
}