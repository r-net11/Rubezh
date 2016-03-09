using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SendEmailArguments
	{
		public SendEmailArguments()
		{
			EMailAddressFromArgument = new Argument();
			EMailAddressToArguments = new List<Argument>();
			EMailTitleArgument = new Argument();
			EMailContentArgument = new Argument();
			EMailAttachedFileArguments = new List<Argument>();
			SmtpArgument = new Argument();
			PortArgument = new Argument();
			LoginArgument = new Argument();
			PasswordArgument = new Argument();
		}

		[DataMember]
		public Argument EMailAddressFromArgument { get; set; }

		[DataMember]
		public List<Argument> EMailAddressToArguments { get; set; }

		[DataMember]
		public Argument EMailTitleArgument { get; set; }

		[DataMember]
		public Argument EMailContentArgument { get; set; }

		[DataMember]
		public List<Argument> EMailAttachedFileArguments { get; set; }

		[DataMember]
		public Argument SmtpArgument { get; set; }

		[DataMember]
		public Argument PortArgument { get; set; }

		[DataMember]
		public Argument LoginArgument { get; set; }

		[DataMember]
		public Argument PasswordArgument { get; set; }
	}
}