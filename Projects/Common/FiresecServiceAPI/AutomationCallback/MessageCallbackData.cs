using System.Runtime.Serialization;

namespace StrazhAPI.AutomationCallback
{
	[DataContract]
	public class MessageCallbackData : AutomationCallbackData
	{
		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }

		[DataMember]
		public bool WithConfirmation { get; set; }
	}
}