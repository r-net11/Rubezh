using System;
using System.Runtime.Serialization;

namespace StrazhAPI.AutomationCallback
{
	[DataContract]
	public class AutomationCallbackResult
	{
		public AutomationCallbackResult()
		{
		}

		[DataMember]
		public Guid CallbackUID { get; set; }

		[DataMember]
		public AutomationCallbackType AutomationCallbackType { get; set; }

		[DataMember]
		public AutomationCallbackData Data { get; set; }
	}
}