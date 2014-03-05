using System.Runtime.Serialization;

namespace FiresecAPI
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