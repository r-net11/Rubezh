using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class CheckPermissionStep : ProcedureStep
	{
		public CheckPermissionStep()
		{
			PermissionArgument = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument PermissionArgument { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.CheckPermission; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { PermissionArgument, ResultArgument }; }
		}
	}
}
