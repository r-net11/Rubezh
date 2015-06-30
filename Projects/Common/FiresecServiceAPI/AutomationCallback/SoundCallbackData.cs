using System;
using System.Runtime.Serialization;

namespace FiresecAPI.AutomationCallback
{
	[DataContract]
	public class SoundCallbackData : AutomationCallbackData
	{
		[DataMember]
		public Guid SoundUID { get; set; }
	}
}
