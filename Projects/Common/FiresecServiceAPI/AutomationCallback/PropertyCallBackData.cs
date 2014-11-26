using System;
using System.Runtime.Serialization;
using FiresecAPI.Automation;

namespace FiresecAPI.AutomationCallback
{
	[DataContract]
	public class PropertyCallBackData : AutomationCallbackData
	{
		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public Guid ObjectUid { get; set; }
	}
}
