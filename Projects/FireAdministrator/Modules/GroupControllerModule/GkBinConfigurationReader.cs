using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Common.GK;
using System.Diagnostics;
using FiresecClient;
using System.IO;

namespace GKModule
{
	public class GkBinConfigurationReader
	{
		public XDeviceConfiguration DeviceConfiguration;
		Dictionary<ushort, XDevice> ControllerDevices;
		XDevice GkDevice;

		public void ReadConfiguration(XDevice gkDevice)
		{
			ControllerDevices = new Dictionary<ushort, XDevice>();
			DeviceConfiguration = new XDeviceConfiguration();
			var rootDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
			var rootDevice = new XDevice()
			{
				Driver = rootDriver,
				DriverUID = rootDriver.UID
			};
			DeviceConfiguration.RootDevice = rootDevice;

			LoadingService.SaveShowProgress("Перевод ГК в технологический режим", 1);
			BinConfigurationWriter.GoToTechnologicalRegime(gkDevice);

			LoadingService.SaveShowProgress("Чтение базы данных объектов ГК", ushort.MaxValue + 1);

			ushort descriptorNo = 0;
			while (true)
			{
				descriptorNo++;
				LoadingService.SaveDoStep("Чтение базы данных объектов ГК " + descriptorNo);

				byte packNo = 1;
				var descriptorNoBytes = new List<byte>(BitConverter.GetBytes(descriptorNo));
				var data = new List<byte>(descriptorNoBytes);
				data.Add(packNo);
				var sendResult = SendManager.Send(gkDevice, 3, 19, ushort.MaxValue, data);
				var bytes = sendResult.Bytes;
				if (sendResult.HasError || bytes.Count == 0)
					break;

				if (bytes.Count < 5)
					break;

				var inputDescriptorNo = BytesHelper.SubstructShort(bytes, 0);
				var inputPackNo = bytes[2];
				if (bytes[3] == 0xff && bytes[4] == 0xff)
					break;

				Parce(bytes.Skip(3).ToList(), descriptorNo);
			}

			LoadingService.SaveDoStep("Перевод ГК в рабочий режим");
			BinConfigurationWriter.GoToWorkingRegime(gkDevice);
			LoadingService.SaveClose();

			XManager.UpdateConfiguration();
		}

		void Parce(List<byte> bytes, int descriptorNo)
		{
			//Trace.WriteLine(BytesHelper.BytesToString(bytes));

			var internalType = BytesHelper.SubstructShort(bytes, 0);
			var controllerAdress = BytesHelper.SubstructShort(bytes, 2);
			var adressOnController = BytesHelper.SubstructShort(bytes, 4);
			var physicalAdress = BytesHelper.SubstructShort(bytes, 6);

			var letters = new List<byte>();
			for (int i = 8; i <= 28; i++)
			{
				byte letter = (byte)bytes[i];
				letters.Add(letter);
			}
			
			var memoryStream = new MemoryStream(letters.ToArray());
			var description = memoryStream.ToString();
			description = Encoding.GetEncoding(1251).GetString(letters.ToArray());
			//var description = ""//new String(letters.ToArray());

			var driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverTypeNo == internalType);
			if (driver != null)
			{
				if (driver.DriverType == XDriverType.GK && descriptorNo > 1)
					driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
				if (driver.DriverType == XDriverType.GKIndicator && descriptorNo > 14)
					driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.KAUIndicator);

				var device = new XDevice()
				{
					Driver = driver,
					DriverUID = driver.UID,
					IntAddress = (byte)(physicalAdress % 256),
					ShleifNo = (byte)(physicalAdress / 256 + 1),
				};
				if (driver.DriverType == XDriverType.GK)
				{
					ControllerDevices.Add(controllerAdress, device);
					DeviceConfiguration.RootDevice.Children.Add(device);
					GkDevice = device;
				}
				if (driver.DriverType == XDriverType.KAU)
				{
					device.IntAddress = (byte)(controllerAdress % 256);
					var modeProperty = new XProperty()
					{
						Name = "Mode",
						Value = (byte)(controllerAdress / 256)
					};
					device.Properties.Add(modeProperty);
					ControllerDevices.Add(controllerAdress, device);
					GkDevice.Children.Add(device);
				}
				if(driver.DriverType != XDriverType.GK && driver.DriverType != XDriverType.KAU)
				{
					var controllerDevice = ControllerDevices.FirstOrDefault(x => x.Key == controllerAdress);
					if (controllerDevice.Value != null)
					{
						controllerDevice.Value.Children.Add(device);
					}
				}
				DeviceConfiguration.Devices.Add(device);
				return;
			}
			if (internalType == 0x100)
			{
				var zone = new XZone()
				{
					Name = description
				};
				DeviceConfiguration.Zones.Add(zone);
				return;
			}
			if (internalType == 0x106)
			{
				var direction = new XDirection()
				{
					Name = description
				};
				DeviceConfiguration.Directions.Add(direction);
				return;
			}
			return;
		}
	}
}