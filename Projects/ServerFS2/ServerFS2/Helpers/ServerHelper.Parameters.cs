using System;
using System.Collections.Generic;
using System.Linq;
using ServerFS2.Helpers;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static void GetDeviceParameters(Device device)
		{
			var allAUProperties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			var properties = RemoveDublicateProperties(allAUProperties);
			foreach (var property in properties)
			{
				var result = SendCodeToPanel(device.Parent, 0x02, 0x53, 0x02, MetadataHelper.GetIdByUid(device.Driver.UID),
				device.AddressOnShleif, 0x00, property.No, 0x00, 0x00, device.ShleifNo - 1);
				foreach (var prop in allAUProperties.FindAll(x => x.No == result[4]))
				{
					var value = ParametersHelper.CreateProperty(result[5] * 256 + result[6], prop);
					var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == value.Name);
					deviceProperty.Value = value.Value;
				}
			}
		}

		public static void SetDeviceParameters(Device device)
		{
			var properties = RemoveDublicateProperties(device.Driver.Properties.FindAll(x => x.IsAUParameter));
			foreach (var property in properties)
			{
				var value = Convert.ToInt32(ParametersHelper.SetConfigurationParameters(property, device));
				SendCodeToPanel(device.Parent, 0x02, 0x53, 0x03, MetadataHelper.GetIdByUid(device.Driver.UID), device.AddressOnShleif,
				0x00, property.No, value % 256, value / 256, device.ShleifNo - 1);
			}
		}

		#region Удаляем из списка все параметры, коды которых уже есть в этом списке (чтобы не дублировать запрос)
		static List<DriverProperty> RemoveDublicateProperties(List<DriverProperty> properties)
		{
			var properties1 = properties;
			var properties2 = properties;
			properties.RemoveAll(x => properties1.FirstOrDefault(z => (properties2.IndexOf(x) > properties1.IndexOf(z)) && (z.No == x.No)) != null);
			return properties;
		}
		#endregion
	}
}