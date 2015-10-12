using System;
using System.Runtime.Serialization;
using RubezhAPI.Automation;

namespace RubezhAPI.AutomationCallback
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
