using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class AutomationSchedule
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid Uid { get; set; }
	}
}
