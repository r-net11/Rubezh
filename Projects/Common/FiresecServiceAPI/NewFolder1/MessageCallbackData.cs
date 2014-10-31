using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.AutomationCallback
{
	[DataContract]
	public class MessageCallbackData
	{
		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }
	}
}
