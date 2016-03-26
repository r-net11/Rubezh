using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class CloseDialogStep : UIStep
	{
		public CloseDialogStep()
		{
			WindowIDArgument = new Argument();
			LayoutFilter.Add(System.Guid.Empty);
		}

		[DataMember]
		public Argument WindowIDArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.CloseDialog; } }
	}
}
