using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ForeachArguments
	{
		public ForeachArguments()
		{
			ListParameter = new Variable();
			ItemParameter = new Variable();
		}

		[DataMember]
		public Variable ListParameter { get; set; }

		[DataMember]
		public Variable ItemParameter { get; set; }
	}
}
