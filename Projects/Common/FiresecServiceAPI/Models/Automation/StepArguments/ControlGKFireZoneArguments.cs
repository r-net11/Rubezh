using System;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlGKFireZoneArguments
	{
		public ControlGKFireZoneArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid ZoneUid { get; set; }
	}
}