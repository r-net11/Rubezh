using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Sound
	{
		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public string SoundName { get; set; }

		[DataMember]
		public BeeperType BeeperType { get; set; }

		[DataMember]
		public bool IsContinious { get; set; }
	}
}