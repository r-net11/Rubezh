using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RubezhAPI.Models
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
			//	zipConfigurationItems.Add(new ZipConfigurationItem(zipConfigurationItem.Name, zipConfigurationItem.MajorVersion, zipConfigurationItem.MinorVersion));
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
			var names = new List<string>();
			names.Add("PlansConfiguration.xml");
			names.Add("SystemConfiguration.xml");
			names.Add("DriversConfiguration.xml");
			names.Add("DeviceConfiguration.xml");
			names.Add("DeviceLibraryConfiguration.xml");
			names.Add("GKDeviceConfiguration.xml");
			names.Add("GKDeviceLibraryConfiguration.xml");
			names.Add("SKDConfiguration.xml");
			names.Add("SKDLibraryConfiguration.xml");
			names.Add("LayoutsConfiguration.xml");
			return names;
		}
	}
}