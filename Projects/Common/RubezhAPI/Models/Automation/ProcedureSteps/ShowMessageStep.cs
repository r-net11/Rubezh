using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ShowMessageStep : UIStep
	{
		public ShowMessageStep()
		{
			MessageArgument = new Argument();
			ConfirmationValueArgument = new Argument();
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
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }

		[DataMember]
		public bool WithConfirmation { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ShowMessage; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { MessageArgument, ConfirmationValueArgument }; }
		}
	}
}