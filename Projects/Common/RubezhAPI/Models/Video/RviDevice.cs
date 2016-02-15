using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class RviDevice
	{
		public RviDevice()
		{
			RviChannels = new List<RviChannel>();
		}
		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public string Ip { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public List<RviChannel> RviChannels { get; set; }
		public RviStatus Status { get; set; }
		public event Action StatusChanged;
		public void OnStatusChanged()
		{
			if (StatusChanged != null)
				StatusChanged();
		}
	}
}