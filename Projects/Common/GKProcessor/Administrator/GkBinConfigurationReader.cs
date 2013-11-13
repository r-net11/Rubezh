//#define LOCALCONFIG
//#define SETCONFIGTOFILE
using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using GKProcessor;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Common;

namespace GKProcessor
{
	public class GkBinConfigurationReader
	{
		Dictionary<ushort, XDevice> ControllerDevices;
		XDevice GkDevice;
		string IpAddress;
		public XDeviceConfiguration DeviceConfiguration;
		
#if !LOCALCONFIG
		public bool ReadConfiguration(XDevice gkDevice)
		{
			IpAddress = gkDevice.GetGKIpAddress();
			ControllerDevices = new Dictionary<ushort, XDevice>();
			DeviceConfiguration = new XDeviceConfiguration();
			var rootDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
			DeviceConfiguration.RootDevice = new XDevice()
			{
				Driver = rootDriver,
				DriverUID = rootDriver.UID
			};
			LoadingService.SaveShowProgress("Перевод ГК в технологический режим", 1);
			BinConfigurationWriter.GoToTechnologicalRegime(gkDevice);

			LoadingService.SaveShowProgress("Чтение базы данных объектов ГК", 50000);

			ushort descriptorNo = 0;
#if SETCONFIGTOFILE
			var allBytes = new List<List<byte>>();
#endif
			while (true)
			{
				descriptorNo++;
				LoadingService.SaveDoStep("Чтение базы данных объектов ГК " + descriptorNo);

				const byte packNo = 1;
				var data = new List<byte>(BitConverter.GetBytes(descriptorNo)) {packNo};
				var sendResult = SendManager.Send(gkDevice, 3, 19, ushort.MaxValue, data);
				var bytes = sendResult.Bytes;
#if SETCONFIGTOFILE
				allBytes.Add(bytes);
#endif
				if (sendResult.HasError || bytes.Count == 0)
				{
					MessageBoxService.ShowError("Возникла ошибка при чтении объекта " + descriptorNo);
					LoadingService.SaveClose();
					return false;
				}
				if (bytes.Count < 5)
					break;

				var inputDescriptorNo = BytesHelper.SubstructShort(bytes, 0);
				var inputPackNo = bytes[2];
				if (bytes[3] == 0xff && bytes[4] == 0xff)
					break;

				try
				{
					Parce(bytes.Skip(3).ToList(), descriptorNo);
				}
				catch (Exception e)
				{
					Logger.Error(e, "GkBinConfigurationReader.ReadConfiguration");
				}
			}
#if SETCONFIGTOFILE
			/* Опция включения записи конфигурации в файл */
			BytesHelper.BytesToFile("GKConfiguration.txt", allBytes);
#endif
			LoadingService.SaveDoStep("Перевод ГК в рабочий режим");
			if (!BinConfigurationWriter.GoToWorkingRegime(gkDevice))
			{
				MessageBoxService.ShowError("Не удалось перевести устройство в рабочий режим в заданное время");
			}
			LoadingService.SaveClose();
			XManager.UpdateConfiguration();
			return true;
		}
#endif
#if LOCALCONFIG
		#region Чтение конфигурации из байтового потока
		public void ReadConfiguration(XDevice device)
		{
			IpAddress = device.GetGKIpAddress();
			var allbytes = BytesHelper.BytesFromFile("GKConfiguration.txt");
			ControllerDevices = new Dictionary<ushort, XDevice>();
			DeviceConfiguration = new XDeviceConfiguration();
			var rootDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
			var rootDevice = new XDevice()
			{
				Driver = rootDriver,
				DriverUID = rootDriver.UID
			};
			DeviceConfiguration.RootDevice = rootDevice;
			ushort descriptorNo = 0;
			int count = 0;
			while (true)
			{
				descriptorNo++;
				byte packNo = 1;
				var descriptorNoBytes = new List<byte>(BitConverter.GetBytes(descriptorNo));
				var data = new List<byte>(descriptorNoBytes);
				data.Add(packNo);
				var bytes = allbytes[count];
				count++;
				if (bytes.Count < 5)
					break;
				if (bytes[3] == 0xff && bytes[4] == 0xff)
					break;
				Parce(bytes.Skip(3).ToList(), descriptorNo);
			}
			XManager.UpdateConfiguration();
		}
		#endregion
#endif
		void Parce(List<byte> bytes, int descriptorNo)
		 {
			var internalType = BytesHelper.SubstructShort(bytes, 0);
			var controllerAdress = BytesHelper.SubstructShort(bytes, 2);
			var adressOnController = BytesHelper.SubstructShort(bytes, 4);
			var physicalAdress = BytesHelper.SubstructShort(bytes, 6);
			if(internalType == 0)
				return;
			var description = BytesHelper.BytesToStringDescription(bytes);
			var driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == internalType);
			if (driver != null)
			{
				if (driver.DriverType == XDriverType.GK && descriptorNo > 1)
				{
					driver = XManager.Drivers.FirstOrDefault(x => x.IsKauOrRSR2Kau);
					if (bytes[0x3a] == 1)
						driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU);
				}
				if (driver.DriverType == XDriverType.GKIndicator && descriptorNo > 14)
					driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAUIndicator);
				var device = new XDevice()
				{
					Driver = driver,
					DriverUID = driver.UID,
					IntAddress = (byte)(physicalAdress % 256),
					ShleifNo = (byte)(physicalAdress / 256 + 1),
				};
				if (driver.DriverType == XDriverType.GK)
				{
					var ipAddressProperty = new XProperty()
					{
						Name = "IPAddress",
						StringValue = IpAddress
					};
					device.Properties.Add(ipAddressProperty);
					device.Parent = XManager.Devices.FirstOrDefault(x => x.DriverType == XDriverType.System);
					ControllerDevices.Add(controllerAdress, device);
					DeviceConfiguration.RootDevice.Children.Add(device);
					GkDevice = device;
				}
				if (driver.IsKauOrRSR2Kau)
				{
					device.IntAddress = (byte)(controllerAdress % 256);
					var modeProperty = new XProperty()
					{
						Name = "Mode",
						Value = (byte)(controllerAdress / 256)
					};
					device.DeviceProperties.Add(modeProperty);
					device.Parent = GkDevice;
					ControllerDevices.Add(controllerAdress, device);
					GkDevice.Children.Add(device);
				}
				if (driver.DriverType != XDriverType.GK && !driver.IsKauOrRSR2Kau && driver.DriverType != XDriverType.System)
				{
					var controllerDevice = ControllerDevices.FirstOrDefault(x => x.Key == controllerAdress);
					if (controllerDevice.Value != null)
					{
						controllerDevice.Value.Children.Add(device);
						device.Parent = controllerDevice.Value;
					}
				}
				DeviceConfiguration.Devices.Add(device);
				return;
			}

			ushort no = 0;
			var descriptionParts = description.Split(new string[1] { "." }, StringSplitOptions.None);
			if (descriptionParts.Count() == 2)
			{
				no = (ushort)Int32.Parse(descriptionParts[0]);
				description = descriptionParts[1];
			}

			if (internalType == 0x100)
			{
				var zone = new XZone()
				{
					Name = description,
					No = no
				};
				DeviceConfiguration.Zones.Add(zone);
				return;
			}
			if (internalType == 0x106)
			{
				var direction = new XDirection()
				{
					Name = description,
					No = no
				};
				DeviceConfiguration.Directions.Add(direction);
				return;
			}
			return;
		}
	}
}