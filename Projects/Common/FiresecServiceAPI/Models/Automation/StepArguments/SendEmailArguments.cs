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
			Host = new ArithmeticParameter();
			Port = new ArithmeticParameter();
			Login = new ArithmeticParameter();
			Password = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter EMailAddress { get; set; }

		[DataMember]
		public ArithmeticParameter EMailTitle { get; set; }

		[DataMember]
		public ArithmeticParameter EMailContent { get; set; }

		[DataMember]
		public ArithmeticParameter Host { get; private set; }

		[DataMember]
		public ArithmeticParameter Port { get; private set; }

		[DataMember]
		public ArithmeticParameter Login { get; private set; }

		[DataMember]
		public ArithmeticParameter Password { get; private set; }
	}
}
