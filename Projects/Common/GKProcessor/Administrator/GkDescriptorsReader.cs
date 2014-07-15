//#define LOCALCONFIG
//#define SETCONFIGTOFILE
using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;

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
			var progressCallback = GKProcessorManager.StartProgress("Чтение конфигурации " + gkDevice.PresentationName, "Проверка связи", 2, true, GKProgressClientType.Administrator);
			var result = DeviceBytesHelper.Ping(gkDevice);
			if (!result)
			{
				Error = "Устройство " + gkDevice.PresentationName + " недоступно";
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
			GKProcessorManager.DoProgress("Перевод ГК в технологический режим", progressCallback);
			if (!DeviceBytesHelper.GoToTechnologicalRegime(gkDevice, progressCallback))
			{
				Error = "Не удалось перевести " + gkDevice.PresentationName + " в технологический режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
				GKProcessorManager.StopProgress(progressCallback);
				return false;
			}
			var gkFileReaderWriter = new GKFileReaderWriter();
			var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(gkDevice);
			if (gkFileReaderWriter.Error != null)
			{
				Error = gkFileReaderWriter.Error;
				GKProcessorManager.StopProgress(progressCallback);
				return false;
			}
			progressCallback = GKProcessorManager.StartProgress("Чтение конфигурации " + gkDevice.PresentationName, "", gkFileInfo.DescriptorsCount, true, GKProgressClientType.Administrator);
			ushort descriptorNo = 0;
#if SETCONFIGTOFILE
			var allBytes = new List<List<byte>>();
#endif
			while (true)
			{
				if (progressCallback.IsCanceled)
				{
					Error = "Операция отменена";
					break;
				}
				descriptorNo++;
				GKProcessorManager.DoProgress("Чтение базы данных объектов ГК " + descriptorNo, progressCallback);
				const byte packNo = 1;
				var data = new List<byte>(BitConverter.GetBytes(descriptorNo)) {packNo};
				var sendResult = SendManager.Send(gkDevice, 3, 19, ushort.MaxValue, data);
				var bytes = sendResult.Bytes;
#if SETCONFIGTOFILE
				allBytes.Add(bytes);
#endif
				if (sendResult.HasError || bytes.Count < 5)
				{
					Error = "Возникла ошибка при чтении объекта " + descriptorNo;
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
			GKProcessorManager.DoProgress("Перевод ГК в рабочий режим", progressCallback);
			if (!DeviceBytesHelper.GoToWorkingRegime(gkDevice, progressCallback))
			{
				Error = "Не удалось перевести устройство в рабочий режим в заданное время";
			}
			GKProcessorManager.StopProgress(progressCallback);
			if(Error != null)
				return false;
			DeviceConfiguration.Update();
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
			GKProcessorManager.OnStartProgress("Чтение конфигурации " + device.PresentationName);
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
			GKProcessorManager.OnStopProgress();
			if (Error != null)
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
			if (internalType == 0x70)
			{
				if (description[0] == 'П')
					driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.FirePump);
				if (description[0] == 'Ж')
					driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.JockeyPump);
				if (description[0] == 'Д')
					driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.DrainagePump);
			}
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

			if (internalType == 0x100 || internalType == 0x106 || internalType == 0x108)
			{
				var isMPT = false;
				var isPumpStation = false;
				ushort no = 0;

				try
				{
					if (description.StartsWith("MПТ."))
					{
						isMPT = true;
					}
					else
					{
						if (description[0] == '0')
							isPumpStation = true;
						no = (ushort)Int32.Parse(description.Substring(0, description.IndexOf(".")));
					}
					description = description.Substring(description.IndexOf(".") + 1);
				}
				catch
				{
					Error = "Невозможно получить номер объекта с дескриптором " + descriptorNo;
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
					if (isPumpStation)
					{
						var pumpStation = new XPumpStation()
						{
							Name = description,
							No = no,
							GkDatabaseParent = GkDevice
						};
						DeviceConfiguration.PumpStations.Add(pumpStation);
					}
					else if (isMPT)
					{
						var mpt = new XMPT()
						{
							Name = description,
							GkDatabaseParent = GkDevice
						};
						DeviceConfiguration.MPTs.Add(mpt);
					}
					else
					{
						var direction = new XDirection
						{
							Name = description,
							No = no,
							GkDatabaseParent = GkDevice
						};
						DeviceConfiguration.Directions.Add(direction);
					}
					return true;
				}
				if (internalType == 0x108)
				{
					var guardZone = new XGuardZone
					{
						Name = description,
						No = no,
						GkDatabaseParent = GkDevice
					};
					DeviceConfiguration.GuardZones.Add(guardZone);
					return true;
				}
			}
			return true;
		}
	}
}