using FiresecAPI.Automation;
using System;
using System.Runtime.Serialization;

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