using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract, Serializable]
	public class AutomationSound
	{
		public AutomationSound()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid Uid { get; set; }
	}
}