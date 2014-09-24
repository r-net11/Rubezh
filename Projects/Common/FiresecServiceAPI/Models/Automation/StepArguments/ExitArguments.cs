using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExitArguments
	{
		public ExitArguments()
		{
			ExitCodeParameter = new Argument();
		}

		[DataMember]
		public Argument ExitCodeParameter { get; set; }
	}
}