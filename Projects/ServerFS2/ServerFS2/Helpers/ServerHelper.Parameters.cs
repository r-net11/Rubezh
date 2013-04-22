using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ClientFS2.ConfigurationWriter;
using FiresecAPI;
using FiresecAPI.Models;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static void GetDeviceParameters(Device device)
		{
			var bytesList = new List<List<byte>>();
			var properties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			var properties1 = properties;
			var properties2 = properties;
			properties.RemoveAll(x => properties1.FirstOrDefault(z => (properties2.IndexOf(x) > properties1.IndexOf(z)) && (z.No == x.No)) != null); // Удаляем из списка все параметры, коды которых уже есть в этом списке (чтобы не дублировать запрос)
			foreach (var property in properties)
			{
				if (Progress != null)
				{
					var index = properties.IndexOf(property);
					var max = properties.Count - 1;
					Progress(index, max, "");
				}
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.Parent.IntAddress + 2));
				bytes.Add((byte)device.Parent.IntAddress);
				bytes.Add(0x02);
				bytes.Add(0x53);
				bytes.Add(0x02);
				bytes.Add((byte)MetadataHelper.GetIdByUid(device.Driver.UID));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x00);
				bytes.Add(property.No);
				bytes.Add(0x00);
				bytes.Add(0x00);
				bytes.Add(Convert.ToByte(device.IntAddress / 256 - 1));
				bytesList.Add(bytes);
			}
			var results = SendCode(bytesList, 3000, 300);
			foreach (var result in results.Result)
			{
				properties = device.Driver.Properties.FindAll(x => x.No == result.Data[11]);
				foreach (var property in properties)
				{
					var value = ParametersHelper.CreateProperty(result.Data[12] * 256 + result.Data[13], property);
					var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == value.Name);
					deviceProperty.Value = value.Value;
				}
			}
		}

		public static void SetDeviceParameters(Device device)
		{
			var bytesList = new List<List<byte>>();
			var properties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			var properties1 = properties;
			var properties2 = properties;
			properties.RemoveAll(x => properties1.FirstOrDefault(z => (properties2.IndexOf(x) > properties1.IndexOf(z)) && (z.No == x.No)) != null); // Удаляем из списка все параметры, коды которых уже есть в этом списке (чтобы не дублировать запрос)
			foreach (var property in properties)
			{
				if ((property == null) || (!property.IsAUParameter))
					continue;
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.Parent.IntAddress + 2));
				bytes.Add((byte)device.Parent.IntAddress);
				bytes.Add(0x02);
				bytes.Add(0x53);
				bytes.Add(0x03);
				bytes.Add((byte)MetadataHelper.GetIdByUid(device.Driver.UID));
				bytes.Add(Convert.ToByte(device.AddressOnShleif));
				bytes.Add(0x00);
				bytes.Add(property.No);
				var value = Convert.ToInt32(ParametersHelper.SetConfigurationParameters(property, device));
				bytes.Add(Convert.ToByte(value % 256));
				bytes.Add(Convert.ToByte(value / 256));
				bytes.Add(Convert.ToByte(device.AddressOnShleif - 1));
				bytesList.Add(bytes);
			}
			SendCode(bytesList, 3000, 300);
		}
	}
}