using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ShowMessageArguments : UIArguments
	{
		public ShowMessageArguments()
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
		public bool IsModalWindow { get; set; }

		[DataMember]
		public bool WithConfirmation { get; set; }
	}
}