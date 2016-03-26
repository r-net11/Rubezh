using RubezhAPI.GK;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlGKDeviceStep : ProcedureStep
	{
		public ControlGKDeviceStep()
		{
			GKDeviceArgument = new Argument();
		}

		[DataMember]
		public Argument GKDeviceArgument { get; set; }

		[DataMember]
		public GKStateBit Command { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlGKDevice; } }
	}
}