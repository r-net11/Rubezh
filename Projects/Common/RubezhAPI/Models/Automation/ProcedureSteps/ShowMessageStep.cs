using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ShowMessageStep : UIStep
	{
		public ShowMessageStep()
		{
			MessageArgument = new Argument();
			ConfirmationValueArgument = new Argument();
			LayoutFilter.Add(Guid.Empty);
		}

		[DataMember]
		public Argument MessageArgument { get; set; }

		[DataMember]
		public Argument ConfirmationValueArgument { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }

		[DataMember]
		public bool WithConfirmation { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ShowMessage; } }
	}
}