using System;
using System.Runtime.Serialization;
using StrazhAPI.Automation;

namespace StrazhAPI.AutomationCallback
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