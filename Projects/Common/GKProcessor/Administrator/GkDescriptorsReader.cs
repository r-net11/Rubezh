#define LOCALCONFIG
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
	public class GkDescriptorsReaderBase : DescriptorReaderBase
	{
		Dictionary<ushort, XDevice> ControllerDevices;
		XDevice GkDevice;
		string IpAddress;
		
#if !LOCALCONFIG
		override public bool ReadConfiguration(XDevice gkDevice)
		{
			var result = DeviceBytesHelper.Ping(gkDevice);
			if (!result)
			{
				ParsingError = "Устройство " + gkDevice.PresentationDriverAndAddress + " недоступно";
				return false;
			}
			IpAddress = gkDevice.GetGKIpAddress();
			ControllerDevices = new Dictionary<ushort, XDevice>();
			DeviceConfiguration = new XDeviceConfiguration();
			var rootDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
			DeviceConfiguration.RootDevice = new XDevice
			{
				Driver = rootDriver,
				DriverUID = rootDriver.UID
			};
			LoadingService.Show("Чтение конфигурации " + gkDevice.PresentationDriverAndAddress, "Перевод ГК в технологический режим", 50000, true);
			GkDescriptorsWriter.GoToTechnologicalRegime(gkDevice);
			LoadingService.Show("Чтение конфигурации " + gkDevice.PresentationDriverAndAddress, "", 50000, true);
			ushort descriptorNo = 0;
#if SETCONFIGTOFILE
			var allBytes = new List<List<byte>>();
#endif
			while (true)
			{
				if (LoadingService.IsCanceled)
				{
					ParsingError = "Операция отменена";
					break;
				}
				descriptorNo++;
				LoadingService.SaveDoStep("Чтение базы данных объектов ГК " + descriptorNo);
				const byte packNo = 1;
				var data = new List<byte>(BitConverter.GetBytes(descriptorNo)) {packNo};
				var sendResult = SendManager.Send(gkDevice, 3, 19, ushort.MaxValue, data);
				var bytes = sendResult.Bytes;
#if SETCONFIGTOFILE
				allBytes.Add(bytes);
#endif
				if (sendResult.HasError || bytes.Count < 5)
				{
					ParsingError = "Возникла ошибка при чтении объекта " + descriptorNo;
					break;
				}

				if (bytes[3] == 0xff && bytes[4] == 0xff)
					break;
				
				if (!Parse(bytes.Skip(3).ToList(), descriptorNo))
					break;
			}
#if SETCONFIGTOFILE
			/* Опция включения записи конфигурации в файл */
			BytesHelper.BytesToFile("GKConfiguration.txt", allBytes);
#endif
			LoadingService.SaveDoStep("Перевод ГК в рабочий режим");
			if (!DeviceBytesHelper.GoToWorkingRegime(gkDevice))
			{
				ParsingError = "Не удалось перевести устройство в рабочий режим в заданное время";
			}
			LoadingService.SaveClose();
			if(ParsingError != null)
				return false;
			DeviceConfiguration.Update();
			XManager.UpdateGKPredefinedName(GkDevice);
			return true;
		}
#endif
#if LOCALCONFIG
		#region Чтение конфигурации из байтового потока
		override public bool ReadConfiguration(XDevice device)
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
			LoadingService.Show("Чтение конфигурации " + device.PresentationDriverAndAddress);
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
				if (!Parse(bytes.Skip(3).ToList(), descriptorNo))
					break;
			}
			LoadingService.SaveClose();
			if (!String.IsNullOrEmpty(ParsingError))
			{
				return false;
			}
			DeviceConfiguration.Update();
			XManager.UpdateGKPredefinedName(GkDevice);
			return true;
		}
		#endregion
#endif
		bool Parse(List<byte> bytes, int descriptorNo)
		 {
			var internalType = BytesHelper.SubstructShort(bytes, 0);
			var controllerAdress = BytesHelper.SubstructShort(bytes, 2);
			var adressOnController = BytesHelper.SubstructShort(bytes, 4);
			var physicalAdress = BytesHelper.SubstructShort(bytes, 6);
			if(internalType == 0)
				return true;
			var description = BytesHelper.BytesToStringDescription(bytes);
			var driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == internalType);
			if (driver != null)
			{
				if (driver.DriverType == XDriverType.GK && descriptorNo > 1)
				{
					driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
					if (bytes[0x3a] == 1)
					{
						driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU);
					}
				}
				if (driver.DriverType == XDriverType.GKIndicator && descriptorNo > 14)
					driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAUIndicator);

				var shleifNo = (byte)(physicalAdress / 256) + 1;
				var device = new XDevice
				{
					Driver = driver,
					DriverUID = driver.UID,
					IntAddress = (byte)(physicalAdress % 256),
				};
				if (driver.DriverType == XDriverType.GK)
				{
					device.Properties.Add(new XProperty{Name = "IPAddress",StringValue = IpAddress});
					ControllerDevices.Add(controllerAdress, device);
					DeviceConfiguration.RootDevice.Children.Add(device);
					GkDevice = device;
				}
				if (driver.IsKauOrRSR2Kau)
				{
					device.IntAddress = (byte)(controllerAdress % 256);
					var modeProperty = new XProperty
					{
						Name = "Mode",
						Value = (byte)(controllerAdress / 256)
					};
					device.DeviceProperties.Add(modeProperty);
					ControllerDevices.Add(controllerAdress, device);
					GkDevice.Children.Add(device);
					for (int i = 0; i < 8; i++)
					{
						var shleif = new XDevice();
						shleif.Driver = driver.DriverType == XDriverType.KAU ? XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU_Shleif) : XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU_Shleif);
						shleif.DriverUID = shleif.Driver.UID;
						shleif.IntAddress = (byte)(i + 1);
						device.Children.Add(shleif);
					}
				}
				if (driver.DriverType != XDriverType.GK && !driver.IsKauOrRSR2Kau && driver.DriverType != XDriverType.System)
				{
					var controllerDevice = ControllerDevices.FirstOrDefault(x => x.Key == controllerAdress);
					if (controllerDevice.Value != null)
					{
						if(1 <= shleifNo && shleifNo <= 8 && physicalAdress != 0)
						{
							var shleif = controllerDevice.Value.Children.FirstOrDefault(x => (x.DriverType == XDriverType.KAU_Shleif || x.DriverType == XDriverType.RSR2_KAU_Shleif) && x.IntAddress == shleifNo);
							shleif.Children.Add(device);
						}
						else
						{
							if (controllerDevice.Value.Driver.DriverType == XDriverType.GK)
								device.IntAddress = (byte) (controllerDevice.Value.Children.Where(x => !x.Driver.HasAddress).Count() + 2);
							else
								device.IntAddress = (byte)(controllerDevice.Value.Children.Where(x => !x.Driver.HasAddress).Count() + 1);
							controllerDevice.Value.Children.Add(device);
						}
					}
				}
				return true;
			}

			if(internalType == 0x100 || internalType == 0x106)
			{
				ushort no;
				try
				{
					no = (ushort)Int32.Parse(description.Substring(0, description.IndexOf(".")));
					description = description.Substring(description.IndexOf(".") + 1);
				}
				catch
				{
					ParsingError = "Невозможно получить номер объекта с дескриптором " + descriptorNo;
					return false;
				}

				if (internalType == 0x100)
				{
					var zone = new XZone
					{
						Name = description,
						No = no,
						GkDatabaseParent = GkDevice
					};
					DeviceConfiguration.Zones.Add(zone);
					return true;
				}
				if (internalType == 0x106)
				{
					var direction = new XDirection
					{
						Name = description,
						No = no,
						GkDatabaseParent = GkDevice
					};
					DeviceConfiguration.Directions.Add(direction);
					return true;
				}
			}
			return true;
		}
	}
}