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
			EMailAddressParameter = new Argument();
			EMailTitleParameter = new Argument();
			EMailContentParameter = new Argument();
			HostParameter = new Argument();
			PortParameter = new Argument();
			LoginParameter = new Argument();
			PasswordParameter = new Argument();
		}

		[DataMember]
		public Argument EMailAddressParameter { get; set; }

		[DataMember]
		public Argument EMailTitleParameter { get; set; }

		[DataMember]
		public Argument EMailContentParameter { get; set; }

		[DataMember]
		public Argument HostParameter { get; set; }

		[DataMember]
		public Argument PortParameter { get; set; }

		[DataMember]
		public Argument LoginParameter { get; set; }

		[DataMember]
		public Argument PasswordParameter { get; set; }
	}
}