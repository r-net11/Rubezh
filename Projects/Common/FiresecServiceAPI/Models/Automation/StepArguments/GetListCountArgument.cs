using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GetListCountArgument
	{
		public GetListCountArgument()
		{
			ListArgument = new Argument();
			CountArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument CountArgument { get; set; }
	}
}