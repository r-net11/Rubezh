using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Helpers;
using ServerFS2.Service;

namespace ServerFS2
{
	public static class DeviceParametersOperationHelper
	{
		public static List<Property> Get(Device device)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение параметров устройства " + device.PresentationName));
			var allAUProperties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			var properties = allAUProperties;
			foreach (var property in properties)
			{
				var response = USBManager.Send(device.Parent, "Запрос параметра АУ", 0x02, 0x58, 0x02, MetadataHelper.GetIdByUid(device.Driver.UID),
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

		public static void Set(Device device, List<Property> properties)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Запись параметров устройства " + device.PresentationName));
			CopyPropertiesToDevice(device, properties);
			var driverProperties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			foreach (var property in driverProperties)
			{
				var value = Convert.ToInt32(ParametersHelper.SetConfigurationParameters(property, device));
				var loValue = value % 256;
				var hiValue = value / 256;
				if (property.IsFFInLowByte)
				{
					loValue = 0xFF;
				}

				USBManager.Send(device.Parent, "Запись параметра АУ", 0x02, 0x58, 0x03, MetadataHelper.GetIdByUid(device.Driver.UID), device.AddressOnShleif,
				0x00, property.No, loValue, hiValue, device.ShleifNo - 1);
			}
		}

		static void CopyPropertiesToDevice(Device device, List<Property> properties)
		{
			foreach (var deviceProperty in properties)
			{
				var property = device.Properties.FirstOrDefault(x => x.Name == deviceProperty.Name);
				if (property != null)
				{
					property.Value = deviceProperty.Value;
				}
				else
				{
					device.Properties.Add(deviceProperty);
				}
			}
		}
	}
}