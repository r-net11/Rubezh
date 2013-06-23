using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using ServerFS2.Helpers;

namespace ServerFS2
{
	public static class DeviceParametersOperationHelper
	{
		public static List<Property> GetDeviceParameters(Device device)
		{
			var allAUProperties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			var properties = RemoveDublicateProperties(allAUProperties);
			foreach (var property in properties)
			{
				var response = USBManager.Send(device.Parent, 0x02, 0x53, 0x02, MetadataHelper.GetIdByUid(device.Driver.UID),
				device.AddressOnShleif, 0x00, property.No, 0x00, 0x00, device.ShleifNo - 1);
				foreach (var driverProperty in allAUProperties.FindAll(x => x.No == response.Bytes[4]))
				{
					var value = ParametersHelper.CreateProperty(response.Bytes[5] * 256 + response.Bytes[6], driverProperty);
					var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == value.Name);
					deviceProperty.Value = value.Value;
				}
			}
			return device.Properties;
		}

		public static void SetDeviceParameters(Device device, List<Property> properties)
		{
			device.Properties = properties;
			var driverProperties = RemoveDublicateProperties(device.Driver.Properties.FindAll(x => x.IsAUParameter));
			foreach (var property in driverProperties)
			{
				var value = Convert.ToInt32(ParametersHelper.SetConfigurationParameters(property, device));
				USBManager.Send(device.Parent, 0x02, 0x53, 0x03, MetadataHelper.GetIdByUid(device.Driver.UID), device.AddressOnShleif,
				0x00, property.No, value % 256, value / 256, device.ShleifNo - 1);
			}
		}

		static List<DriverProperty> RemoveDublicateProperties(List<DriverProperty> properties)
		{
			var properties1 = properties;
			var properties2 = properties;
			properties.RemoveAll(x => properties1.FirstOrDefault(z => (properties2.IndexOf(x) > properties1.IndexOf(z)) && (z.No == x.No)) != null);
			return properties;
		}
	}
}