using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
