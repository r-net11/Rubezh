using RubezhAPI.Automation;
using System;
using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class PropertyCallBackData : UIAutomationCallbackData
	{
		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public Guid ObjectUid { get; set; }
	}
}
