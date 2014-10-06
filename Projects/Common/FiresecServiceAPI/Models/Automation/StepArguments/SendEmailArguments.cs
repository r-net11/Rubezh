using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SendEmailArguments
	{
		public SendEmailArguments()
		{
			EMailAddressArgument = new Argument();
			EMailTitleArgument = new Argument();
			EMailContentArgument = new Argument();
			HostArgument = new Argument();
			PortArgument = new Argument();
			LoginArgument = new Argument();
			PasswordArgument = new Argument();
		}

		[DataMember]
		public Argument EMailAddressArgument { get; set; }

		[DataMember]
		public Argument EMailTitleArgument { get; set; }

		[DataMember]
		public Argument EMailContentArgument { get; set; }

		[DataMember]
		public Argument HostArgument { get; set; }

		[DataMember]
		public Argument PortArgument { get; set; }

		[DataMember]
		public Argument LoginArgument { get; set; }

		[DataMember]
		public Argument PasswordArgument { get; set; }
	}
}