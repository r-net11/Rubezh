using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ZipConfigurationItemsCollection : VersionedConfiguration
	{
		public ZipConfigurationItemsCollection()
		{
			ZipConfigurationItems = new List<ZipConfigurationItem>();
		}

		[DataMember]
		public List<ZipConfigurationItem> ZipConfigurationItems { get; set; }

		public List<ZipConfigurationItem> GetWellKnownZipConfigurationItems()
		{
			var zipConfigurationItems = new List<ZipConfigurationItem>();
			//foreach (var zipConfigurationItem in ZipConfigurationItems)
			//{
			//    zipConfigurationItems.Add(new ZipConfigurationItem(zipConfigurationItem.Name, zipConfigurationItem.MajorVersion, zipConfigurationItem.MinorVersion));
			//}
			foreach (var name in GetWellKnownNames())
			{
				if (!zipConfigurationItems.Any(x => x.Name == name))
					zipConfigurationItems.Add(new ZipConfigurationItem(name, 1, 1));
			}
			return zipConfigurationItems;
		}

		public static List<string> GetWellKnownNames()
		{
			var names = new List<string>
			{
				"SecurityConfiguration.xml",
				"PlansConfiguration.xml",
				"SystemConfiguration.xml",
				"DriversConfiguration.xml",
				"DeviceConfiguration.xml",
				"DeviceLibraryConfiguration.xml",
				"SKDConfiguration.xml",
				"SKDLibraryConfiguration.xml",
				"LayoutsConfiguration.xml"
			};
			return names;
		}
	}
}