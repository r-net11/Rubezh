using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class RviChannel
	{
		public RviChannel()
		{
			Cameras = new List<Camera>();
		}
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int Number { get; set; }
		[DataMember]
		public int Vendor { get; set; }
		[DataMember]
		public List<Camera> Cameras { get; set; }
	}
}