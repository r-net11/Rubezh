using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class SendMessageArguments
	{
		public SendMessageArguments()
		{

		}

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }

		[DataMember]
		public Guid GlobalVariableUid { get; set; }
		
		[DataMember]
		public VariableType VariableType { get; set; }
	}
}