using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class TimeTrackZone
	{
		[DataMember]
		public SKDZone SKDZone { get; set; }
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public int No { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public bool IsURV { get; set; }
	}
}
