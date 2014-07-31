using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace FiresecAPI
{
	[DataContract]
	public class AutomationCallbackResult
	{
		public AutomationCallbackResult()
		{
		}

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public Guid SoundUID { get; set; }

		[DataMember]
		public AutomationCallbackType AutomationCallbackType { get; set; }
	}

	public enum AutomationCallbackType
	{
		Message,
		Sound
	}
}