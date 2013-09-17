using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.GK;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule
{
	public static class KauBinConfigurationReader
	{
		public static List<XDevice> ReadConfiguration(XDevice kauDevice)
		{
			var devices = new List<XDevice>();

			LoadingService.SaveShowProgress("Перевод КАУ в технологический режим", 1);
			BinConfigurationWriter.GoToTechnologicalRegime(kauDevice);
			var descriptorAddersses = GetDescriptorAddresses(kauDevice);

			LoadingService.SaveShowProgress("Чтение базы данных объектов", descriptorAddersses.Count + 1);
			for(int i = 0; i < descriptorAddersses.Count; i++)
			{
				LoadingService.SaveDoStep("Чтение базы данных объектов. " + i.ToString() + " из " + descriptorAddersses.Count.ToString());
				var device = GetDescriptorInfo(kauDevice, descriptorAddersses[i]);
				devices.Add(device);
			}
			LoadingService.SaveDoStep("Перевод КАУ в рабочий режим");
			BinConfigurationWriter.GoToWorkingRegime(kauDevice);
			LoadingService.SaveClose();

			devices.RemoveAll(x => x.Driver.DriverType == XDriverType.GK || x.Driver.DriverType == XDriverType.GKIndicator);
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
			var parametersOffset = BytesHelper.SubstructShort(bytes, 4);
			var inputDependensesCount = BytesHelper.SubstructShort(bytes, 6);
			for (int i = 0; i < Math.Min((int)inputDependensesCount, 100); i++)
			{
				var inputDependensyNo = BytesHelper.SubstructShort(bytes, 8 + i * 2);
			}
			Trace.WriteLine(descriptorAdderss + " " + BytesHelper.BytesToString(descriptorAdderssesBytes) +
				deviceType.ToString() + " " + address.ToString() + " " + parametersOffset.ToString() + " " + inputDependensesCount.ToString() + " "
				);

			var device = new XDevice();
			device.Driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);
			if (device.Driver.DriverType == XDriverType.GKIndicator)
				device.Driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAUIndicator);
			device.DriverUID = device.Driver.UID;
			device.ShleifNo = (byte)(address / 256 + 1);
			device.IntAddress = (byte)(address % 256);
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