using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Common.GK;
using System.Diagnostics;
using Infrastructure.Common.Windows;
using System.Collections;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;

namespace GKModule
{
	public static class KauBinConfigurationReader
	{
		public static bool ReadConfiguration(XDevice kauDevice)
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

			var deviceConfigurationViewModel = new DeviceConfigurationViewModel(kauDevice, devices);
			return DialogService.ShowModalWindow(deviceConfigurationViewModel);
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

			//parametersOffset = (ushort)(0x10000 + parametersOffset);
			//var parametersOffsetBytes = new List<byte>(BitConverter.GetBytes(descriptorAdderss));
			//data = new List<byte>(parametersOffsetBytes);
			//sendResult = SendManager.Send(kauDevice, 4, 30, 256, data);

			var device = new XDevice();
			device.Driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x=>x.DriverTypeNo == deviceType);
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