using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class MessageCallbackData : AutomationCallbackData
	{
		[DataMember]
		public object Message { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }

		[DataMember]
		public bool WithConfirmation { get; set; }
	}
}
