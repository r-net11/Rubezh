﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class KauDescriptorsReaderBase : DescriptorReaderBase
	{
		GKDevice KauDevice { get; set; }
		List<int> descriptorAddresses;

		override public bool ReadConfiguration(GKDevice kauDevice)
		{
			KauDevice = (GKDevice) kauDevice.Clone();
			KauDevice.Children = new List<GKDevice>();
			descriptorAddresses = new List<int>();
			for (int i = 0; i < 8; i++)
			{
				var shleif = new GKDevice();
				shleif.Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
				shleif.DriverUID = shleif.Driver.UID;
				shleif.IntAddress = (byte)(i + 1);
				KauDevice.Children.Add(shleif);
			}
			DeviceConfiguration = new GKDeviceConfiguration { RootDevice = KauDevice };
			var progressCallback = GKProcessorManager.StartProgress("Чтение конфигурации " + kauDevice.PresentationName, "", descriptorAddresses.Count + 2, true, GKProgressClientType.Administrator);
			GKProcessorManager.DoProgress("Перевод КАУ в технологический режим", progressCallback);
			if (!DeviceBytesHelper.GoToTechnologicalRegime(kauDevice, progressCallback))
				{ Error = "Не удалось перевести КАУ в технологический режим"; return false; }
			GKProcessorManager.DoProgress("Получение дескрипторов устройств", progressCallback);
			if (GetDescriptorAddresses(kauDevice))
			{
				progressCallback.StepCount = descriptorAddresses.Count + 1;
				for (int i = 1; i < descriptorAddresses.Count; i++)
				{
					if (progressCallback.IsCanceled)
					{
						Error = "Операция отменена";
						break;
					}
					GKProcessorManager.DoProgress("Чтение базы данных объектов. " + i + " из " + descriptorAddresses.Count, progressCallback);
					if (!GetDescriptorInfo(kauDevice, descriptorAddresses[i]))
						break;
				}
			}
			GKProcessorManager.DoProgress("Перевод КАУ в рабочий режим", progressCallback);
			DeviceBytesHelper.GoToWorkingRegime(kauDevice, progressCallback);
			GKProcessorManager.StopProgress(progressCallback);
			return String.IsNullOrEmpty(Error);
		}

		bool GetDescriptorInfo(GKDevice kauDevice, int descriptorAdderss)
		{
			var descriptorAdderssesBytes = new List<byte>(BitConverter.GetBytes(descriptorAdderss));
			var data = new List<byte>(descriptorAdderssesBytes);
			var sendResult = SendManager.Send(kauDevice, 4, 31, 256, data);
			var bytes = sendResult.Bytes;
			if (bytes.Count != 256)
			{
				Error = "Длина дескриптора не соответствует нужному значению";
				return false;
			}
			var deviceType = BytesHelper.SubstructShort(bytes, 0);
			var address = BytesHelper.SubstructShort(bytes, 2);
			int shleifNo = (byte)(address / 256 + 1);
			var device = new GKDevice();
			device.Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);
			if ((1 <= shleifNo && shleifNo <= 8) && (address != 0))
			{
				device.DriverUID = device.Driver.UID;
				var shleif = KauDevice.Children.FirstOrDefault(x => (x.DriverType == GKDriverType.RSR2_KAU_Shleif) && x.IntAddress == shleifNo);
				shleif.Children.Add(device);
				device.IntAddress = (byte)(address % 256);
				return true;
			}
			device.Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.KAUIndicator);
			device.DriverUID = device.Driver.UID;
			device.IntAddress = 1;
			KauDevice.Children.Add(device);
			return true;
		}

		bool GetDescriptorAddresses(GKDevice device)
		{
			descriptorAddresses = new List<int>();
			var startaddress = 0x078000;
			while (true)
			{
				byte[] startAddressBytes = BitConverter.GetBytes(startaddress);
				startaddress += 256;

				var data = new List<byte>(startAddressBytes);
				var sendResult = SendManager.Send(device, 4, 31, 256, data);
				if (sendResult.Bytes.Count != 256)
				{
					Error = "Не удалось распознать дескриптор";
					return false;
				}
				for (int i = 0; i < 256 / 4; i++)
				{
					var descriptorAddress = BytesHelper.SubstructInt(sendResult.Bytes, i * 4);
					if (descriptorAddress == -1)
					{
						return true;
					}
					descriptorAddresses.Add(descriptorAddress);
				}
			}
		}
	}
}