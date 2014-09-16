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
			EMailAddressParameter = new ArithmeticParameter();
			EMailTitleParameter = new ArithmeticParameter();
			EMailContentParameter = new ArithmeticParameter();
			HostParameter = new ArithmeticParameter();
			PortParameter = new ArithmeticParameter();
			LoginParameter = new ArithmeticParameter();
			PasswordParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter EMailAddressParameter { get; set; }

		[DataMember]
		public ArithmeticParameter EMailTitleParameter { get; set; }

		[DataMember]
		public ArithmeticParameter EMailContentParameter { get; set; }

		[DataMember]
		public ArithmeticParameter HostParameter { get; private set; }

		[DataMember]
		public ArithmeticParameter PortParameter { get; private set; }

		[DataMember]
		public ArithmeticParameter LoginParameter { get; private set; }

		[DataMember]
		public ArithmeticParameter PasswordParameter { get; private set; }
	}
}
