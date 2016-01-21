using System;
using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class OpcDaTagCallBackData : AutomationCallbackData
	{
		[DataMember]
		public Guid TagUID { get; set; }

		[DataMember]
		public object Value { get; set; }
	}
}
