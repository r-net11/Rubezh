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
			Variable1 = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public string Email{ get; set; }

		[DataMember]
		public ValueType ValueType { get; set; }
	}
}
