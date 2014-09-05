using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExitArguments
	{
		public ExitArguments()
		{
			ExitCode = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter ExitCode { get; set; }
	}
}