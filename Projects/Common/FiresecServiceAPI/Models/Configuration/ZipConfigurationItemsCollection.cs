using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
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

		public List<ZipConfigurationItem> GetWellKnownZipConfigurationItems
		{
			get
			{
				var zipConfigurationItems = new List<ZipConfigurationItem>();
				foreach (var zipConfigurationItem in ZipConfigurationItems)
				{
					zipConfigurationItems.Add(new ZipConfigurationItem(zipConfigurationItem.Name, zipConfigurationItem.MajorVersion, zipConfigurationItem.MinorVersion));
				}
				foreach (var name in GetWellKnownNames())
				{
					if (!zipConfigurationItems.Any(x => x.Name == name))
						zipConfigurationItems.Add(new ZipConfigurationItem(name, 1, 1));
				}
				return zipConfigurationItems;
			}
		}

		static List<string> GetWellKnownNames()
		{
			var names = new List<string>();
			names.Add("SecurityConfiguration.xml");
			names.Add("PlansConfiguration.xml");
			names.Add("SystemConfiguration.xml");
			names.Add("DriversConfiguration.xml");
			names.Add("DeviceConfiguration.xml");
			names.Add("DeviceLibraryConfiguration.xml");
			names.Add("XDeviceConfiguration.xml");
			names.Add("XDeviceLibraryConfiguration.xml");
			return names;
		}
	}
}