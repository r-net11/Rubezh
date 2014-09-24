using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ForeachArguments
	{
		public ForeachArguments()
		{
			ListParameter = new Argument();
			ItemParameter = new Argument();
		}

		[DataMember]
		public Argument ListParameter { get; set; }

		[DataMember]
		public Argument ItemParameter { get; set; }
	}
}
