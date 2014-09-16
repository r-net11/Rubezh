using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExitArguments
	{
		public ExitArguments()
		{
			ExitCodeParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter ExitCodeParameter { get; set; }
	}
}