using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AutomationCallbackResult
	{
		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }

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