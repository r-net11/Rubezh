using System.Runtime.Serialization;

namespace RubezhAPI
{
	[DataContract]
	public class ConfigurationVersion
	{
		[DataMember]
		public int MajorVersion { get; set; }

		[DataMember]
		public int MinorVersion { get; set; }
	}
}