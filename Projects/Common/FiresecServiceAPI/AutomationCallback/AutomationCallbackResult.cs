using System;
using System.Runtime.Serialization;

namespace FiresecAPI.AutomationCallback
{
	[DataContract]
	public class AutomationCallbackResult
	{
		public AutomationCallbackResult()
		{
		}

		[DataMember]
		public Guid ProcedureUID { get; set; }

		[DataMember]
		public AutomationCallbackType AutomationCallbackType { get; set; }

		[DataMember]
		public AutomationCallbackData Data { get; set; }
	}
}