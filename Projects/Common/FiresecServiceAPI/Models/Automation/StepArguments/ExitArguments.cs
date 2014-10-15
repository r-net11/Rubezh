using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract, Serializable]
	public class ExitArguments
	{
		public ExitArguments()
		{
			ExitCodeArgument = new Argument();
		}

		[DataMember]
		public Argument ExitCodeArgument { get; set; }
	}
}