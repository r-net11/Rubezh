using System;
using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class SoundCallbackData : UIAutomationCallbackData
	{
		[DataMember]
		public Guid SoundUID { get; set; }
	}
}
