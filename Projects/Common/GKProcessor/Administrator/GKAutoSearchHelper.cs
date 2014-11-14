using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace GKProcessor
{
	public class GKAutoSearchHelper
	{
		public string Error { get; private set; }

		public GKDeviceConfiguration AutoSearch(GKDevice gkControllerDevice)
		{
			var result = new GKDeviceConfiguration();
			result.RootDevice = GKManager.CopyDevice(gkControllerDevice, false);
			result.RootDevice.Children.RemoveAll(x => x.Driver.IsKauOrRSR2Kau);

			var progressCallback = GKProcessorManager.StartProgress("Автопоиск устройств на " + gkControllerDevice.PresentationName, "Проверка связи", 1, true, GKProgressClientType.Administrator);
			try
			{
				var kauDevices = new List<GKDevice>();

				progressCallback = GKProcessorManager.StartProgress("Автопоиск КАУ на " + gkControllerDevice.PresentationName, "", (int)(128), true, GKProgressClientType.Administrator);
				for (byte i = 1; i < 128; i++)
				{
					if (progressCallback.IsCanceled)
					{ Error = "Операция отменена"; return null; }

					var kauDevice = new GKDevice();
					kauDevice.Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU);
					kauDevice.DriverUID = kauDevice.Driver.UID;
					kauDevice.Parent = gkControllerDevice;
					kauDevice.IntAddress = i;
					kauDevice.Properties.Add(new GKProperty() { Name = "Mode", Value = 0 });
					GKManager.AddAutoCreateChildren(kauDevice);

					var result1 = SendManager.Send(kauDevice, 0, 1, 1);
					if (!result1.HasError)
					{
						var sendResult = SendManager.Send(kauDevice, 2, 12, 32, BytesHelper.ShortToBytes(1));
						if (!sendResult.HasError && sendResult.Bytes.Count == 32)
						{
							var alsParameterByte = sendResult.Bytes[27];
							kauDevice.Properties.Add(new GKProperty() { Name = "als12", Value = (ushort)(alsParameterByte & 0x03) });
							kauDevice.Properties.Add(new GKProperty() { Name = "als34", Value = (ushort)(alsParameterByte & 0x0C) });
							kauDevice.Properties.Add(new GKProperty() { Name = "als56", Value = (ushort)(alsParameterByte & 0x30) });
							kauDevice.Properties.Add(new GKProperty() { Name = "als78", Value = (ushort)(alsParameterByte & 0xC0) });
						}

						kauDevices.Add(kauDevice);
						result.RootDevice.Children.Add(kauDevice);
					}

					GKProcessorManager.DoProgress("Поиск КАУ с адресом " + i, progressCallback);
				}

				foreach (var kauDevice in kauDevices)
				{
					var shleifNos = new List<int>();

					shleifNos.Add(0);
					if (kauDevice.Properties.FirstOrDefault(x => x.Name == "als12").Value == 0)
						shleifNos.Add(1);

					shleifNos.Add(2);
					if (kauDevice.Properties.FirstOrDefault(x => x.Name == "als34").Value == 0)
						shleifNos.Add(3);

					shleifNos.Add(4);
					if (kauDevice.Properties.FirstOrDefault(x => x.Name == "als56").Value == 0)
						shleifNos.Add(5);

					shleifNos.Add(6);
					if (kauDevice.Properties.FirstOrDefault(x => x.Name == "als78").Value == 0)
						shleifNos.Add(7);

					foreach (var shleifNo in shleifNos)
					{
						if (!FindDevicesOnShleif(kauDevice, shleifNo, progressCallback))
							return null;
					}
				}
			}
			catch (Exception e)
			{ Logger.Error(e, "GKDescriptorsWriter.GKAutoSearchHelper"); Error = "Непредвиденная ошибка"; return null; }
			finally
			{
				if (progressCallback != null)
					GKProcessorManager.StopProgress(progressCallback);
			}

			return result;
		}

		bool FindDevicesOnShleif(GKDevice kauDevice, int shleifNo, GKProgressCallback progressCallback)
		{
			var shleifDevice = kauDevice.Children.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif && x.IntAddress == shleifNo + 1);
			progressCallback = GKProcessorManager.StartProgress("Автопоиск на шлейфе " + (shleifNo + 1).ToString() + " устройста " + kauDevice.PresentationName, "", (int)(256), true, GKProgressClientType.Administrator);

			var deviceGroups = new List<DeviceGroup>();
			var devices = new List<GKDevice>();
			for (int address = 1; address <= 255; address++)
			{
				GKProcessorManager.DoProgress("Поиск устройства с адресом " + address, progressCallback);
				if (progressCallback.IsCanceled)
				{ Error = "Операция отменена"; return false; }

				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.Add((byte)address);
				bytes.Add((byte)shleifNo);
				var result2 = SendManager.Send(kauDevice, 3, 0x86, 6, bytes, true, false, 500);
				if (!result2.HasError)
				{
					if (result2.Bytes.Count == 6)
					{
						var driverTypeNo = result2.Bytes[1];
						var serialNo = BytesHelper.SubstructInt(result2.Bytes, 2);
						var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == (ushort)driverTypeNo);
						if (driver != null)
						{
							var device = new GKDevice();
							device.Driver = driver;
							device.DriverUID = driver.UID;
							device.IntAddress = (byte)address;
							devices.Add(device);

							var deviceGroup = deviceGroups.FirstOrDefault(x => x.SerialNo == serialNo);
							if (deviceGroup == null || (serialNo == 0 || serialNo == -1) || (driver.DriverType != GKDriverType.RSR2_AM_1 && driver.DriverType != GKDriverType.RSR2_MAP4 && driver.DriverType != GKDriverType.RSR2_MVK8 && driver.DriverType != GKDriverType.RSR2_RM_1))
							{
								deviceGroup = new DeviceGroup();
								deviceGroup.SerialNo = serialNo;
								deviceGroups.Add(deviceGroup);
							}
							deviceGroup.Devices.Add(device);
						}
					}
				}
				else
				{
					break;
				}
			}

			foreach (var deviceGroup in deviceGroups)
			{
				var firstDeviceInGroup = deviceGroup.Devices.FirstOrDefault();
				if (deviceGroup.Devices.Count > 1)
				{
					GKDriver groupDriver = null;
					if (firstDeviceInGroup.Driver.DriverType == GKDriverType.RSR2_AM_1)
					{
						groupDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_AM_4);
					}
					if (firstDeviceInGroup.Driver.DriverType == GKDriverType.RSR2_MAP4)
					{
						groupDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_MAP4_Group);
					}
					if (firstDeviceInGroup.Driver.DriverType == GKDriverType.RSR2_MVK8)
					{
						groupDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_MVK8_Group);
					}
					if (firstDeviceInGroup.Driver.DriverType == GKDriverType.RSR2_RM_1)
					{
						groupDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_RM_2);
					}

					var groupDevice = new GKDevice();
					groupDevice.Driver = groupDriver;
					groupDevice.DriverUID = groupDriver.UID;
					groupDevice.IntAddress = firstDeviceInGroup.IntAddress;
					foreach (var deviceInGroup in deviceGroup.Devices)
					{
						groupDevice.Children.Add(deviceInGroup);
					}
					shleifDevice.Children.Add(groupDevice);
				}
				else
				{
					shleifDevice.Children.Add(firstDeviceInGroup);
				}
			}
			return true;
		}


		class DeviceGroup
		{
			public DeviceGroup()
			{
				Devices = new List<GKDevice>();
			}
			public int SerialNo { get; set; }
			public List<GKDevice> Devices { get; set; }
		}
	}
}