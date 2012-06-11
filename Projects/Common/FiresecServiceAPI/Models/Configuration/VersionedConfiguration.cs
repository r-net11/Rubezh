using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class VersionedConfiguration
	{
		public VersionedConfiguration()
		{
			Version = new ConfigurationVersion();
		}

		[DataMember]
		public ConfigurationVersion Version { get; set; }
	}
}