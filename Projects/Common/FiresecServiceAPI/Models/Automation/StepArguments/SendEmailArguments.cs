using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class SendEmailArguments
	{
		public SendEmailArguments()
		{
			EMailAddress = new ArithmeticParameter();
			EMailTitle = new ArithmeticParameter();
			EMailContent = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter EMailAddress { get; set; }

		[DataMember]
		public ArithmeticParameter EMailTitle { get; set; }

		[DataMember]
		public ArithmeticParameter EMailContent { get; set; }

		[DataMember]
		public string Email { get; set; }

		[DataMember]
		public string Host { get; set; }

		[DataMember]
		public string Port { get; set; }

		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public string Password { get; set; }
	}
}
