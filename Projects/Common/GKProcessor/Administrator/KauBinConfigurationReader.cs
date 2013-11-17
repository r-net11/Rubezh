using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class KauBinConfigurationReader
	{
		static XDevice KauDevice { get; set; }
		public static List<XDevice> ReadConfiguration(XDevice kauDevice)
		{
			KauDevice = (XDevice) kauDevice.Clone();
			KauDevice.Children = new List<XDevice>();
			for (int i = 0; i < 8; i++)
			{
				var shleif = new XDevice();
				shleif.Driver = KauDevice.DriverType == XDriverType.KAU ? XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU_Shleif) : XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU_Shleif);
				shleif.DriverUID = shleif.Driver.UID;
				shleif.IntAddress = (byte)(i + 1);
				shleif.Parent = KauDevice;
				KauDevice.Children.Add(shleif);
			}

			var devices = new List<XDevice>();
			devices.Add(KauDevice);
			LoadingService.SaveShowProgress("Перевод КАУ в технологический режим", 1);
			BinConfigurationWriter.GoToTechnologicalRegime(kauDevice);
			LoadingService.SaveShowProgress("Получение дескрипторов устройств", 1);
			var descriptorAddersses = GetDescriptorAddresses(kauDevice);

			LoadingService.SaveShowProgress("Чтение базы данных объектов", descriptorAddersses.Count + 1);
			LoadingService.Show("Чтение конфигурации");
			for(int i = 1; i < descriptorAddersses.Count; i++)
			{
				LoadingService.SaveDoStep("Чтение базы данных объектов. " + i + " из " + descriptorAddersses.Count);
				var device = GetDescriptorInfo(kauDevice, descriptorAddersses[i]);
				devices.Add(device);
			}
			LoadingService.SaveDoStep("Перевод КАУ в рабочий режим");
			BinConfigurationWriter.GoToWorkingRegime(kauDevice);
			LoadingService.SaveClose();
			return devices;
		}

		static XDevice GetDescriptorInfo(XDevice kauDevice, int descriptorAdderss)
		{
			var descriptorAdderssesBytes = new List<byte>(BitConverter.GetBytes(descriptorAdderss));
			var data = new List<byte>(descriptorAdderssesBytes);
			var sendResult = SendManager.Send(kauDevice, 4, 31, 256, data);
			var bytes = sendResult.Bytes;
			if (bytes.Count != 256)
			{
				MessageBoxService.ShowError("bytes.Count != 256");
				return null;
			}
			var deviceType = BytesHelper.SubstructShort(bytes, 0);
			var address = BytesHelper.SubstructShort(bytes, 2);
			int shleifNo = (byte)(address / 256 + 1);
			var device = new XDevice();
			device.Driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);
			if ((1 <= shleifNo && shleifNo <= 8) && (address != 0))
			{
				var shleif = KauDevice.Children.FirstOrDefault(x => (x.DriverType == XDriverType.KAU_Shleif || x.DriverType == XDriverType.RSR2_KAU_Shleif) && x.IntAddress == shleifNo);
				shleif.Children.Add(device);
				device.Parent = shleif;
				device.IntAddress = (byte)(address % 256);
			}
			else
			{
				device.Driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAUIndicator);
				device.IntAddress = (byte)(KauDevice.Children.Where(x => !x.Driver.HasAddress).Count() + 1);
				KauDevice.Children.Add(device);
				device.Parent = KauDevice;
			}
			device.DriverUID = device.Driver.UID;
			return device;
		}

		static List<int> GetDescriptorAddresses(XDevice device)
		{
			var descriptorAddersses = new List<int>();
			int startaddress = 0x078000;

			while (true)
			{
				byte[] startAddressBytes = BitConverter.GetBytes(startaddress);
				startaddress += 256;

				var data = new List<byte>(startAddressBytes);
				var sendResult = SendManager.Send(device, 4, 31, 256, data);
				if (sendResult.Bytes.Count != 256)
				{
					MessageBoxService.ShowError("bytes.Count != 256");
					return descriptorAddersses;
				}
				for (int i = 0; i < 256 / 4; i++)
				{
					var descriptorAdderss = BytesHelper.SubstructInt(sendResult.Bytes, i * 4);
					if (descriptorAdderss == -1)
					{
						return descriptorAddersses;
					}
                    descriptorAddersses.Add(descriptorAdderss);
				}
			}
		}
	}
}