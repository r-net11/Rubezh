using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RubezhAPI.GK
{
	[DataContract]
	public class GKPropertyChangedCallback
	{
		public GKPropertyChangedCallback()
		{
			DeviceProperties = new List<GKProperty>();
		}

		[DataMember]
		public List<GKProperty> DeviceProperties { get; set; }

		[DataMember]
		public Guid ObjectUID { get; set; }
	}
}