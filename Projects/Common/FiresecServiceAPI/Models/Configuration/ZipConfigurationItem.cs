using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ZipConfigurationItem
	{
		public ZipConfigurationItem()
		{
		}

		public ZipConfigurationItem(string name, int majorVersion, int minorVersion)
		{
			Name = name;
			MajorVersion = majorVersion;
			MinorVersion = minorVersion;
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int MajorVersion { get; set; }
		[DataMember]
		public int MinorVersion { get; set; }
	}
}