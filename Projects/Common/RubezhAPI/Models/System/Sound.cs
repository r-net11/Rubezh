using System.Runtime.Serialization;
using RubezhAPI.GK;

namespace RubezhAPI.Models
{
	[DataContract]
	public class Sound
	{
		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public string SoundName { get; set; }

		[DataMember]
		public bool IsContinious { get; set; }

		[DataMember]
		public SoundType Type{ get; set; }
	}
}