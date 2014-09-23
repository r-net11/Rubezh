using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExitArguments
	{
		public ExitArguments()
		{
			ExitCodeParameter = new Variable();
		}

		[DataMember]
		public Variable ExitCodeParameter { get; set; }
	}
}