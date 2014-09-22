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
			EMailAddressParameter = new Variable();
			EMailTitleParameter = new Variable();
			EMailContentParameter = new Variable();
			HostParameter = new Variable();
			PortParameter = new Variable();
			LoginParameter = new Variable();
			PasswordParameter = new Variable();
		}

		[DataMember]
		public Variable EMailAddressParameter { get; set; }

		[DataMember]
		public Variable EMailTitleParameter { get; set; }

		[DataMember]
		public Variable EMailContentParameter { get; set; }

		[DataMember]
		public Variable HostParameter { get; private set; }

		[DataMember]
		public Variable PortParameter { get; private set; }

		[DataMember]
		public Variable LoginParameter { get; private set; }

		[DataMember]
		public Variable PasswordParameter { get; private set; }
	}
}
