using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using Common.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule
{
	public class GkBinConfigurationReader
	{
		public XDeviceConfiguration DeviceConfiguration;
		Dictionary<ushort, XDevice> ControllerDevices;
		XDevice GkDevice;
		string IpAddress;

		public void ReadConfiguration(XDevice gkDevice)
		{
			IpAddress = gkDevice.GetGKIpAddress();
			ControllerDevices = new Dictionary<ushort, XDevice>();
			DeviceConfiguration = new XDeviceConfiguration();
			var rootDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
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
			var allBytes = new List<List<byte>>();
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
				allBytes.Add(bytes);
				if (sendResult.HasError || bytes.Count == 0)
					break;

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
			BytesHelper.BytesToFile("GKConfiguration.txt", allBytes);
			LoadingService.SaveDoStep("Перевод ГК в рабочий режим");
			if (!BinConfigurationWriter.GoToWorkingRegime(gkDevice))
			{
				MessageBoxService.ShowError("Не удалось перевести устройство в рабочий режим в заданное время");
			}
			LoadingService.SaveClose();

			XManager.UpdateConfiguration();
		}

		//public void ReadConfiguration(XDevice device)
		//{
		//    var allbytes = BytesHelper.BytesFromFile("GKConfiguration.txt");
		//    ControllerDevices = new Dictionary<ushort, XDevice>();
		//    DeviceConfiguration = new XDeviceConfiguration();
		//    var rootDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
		//    var rootDevice = new XDevice()
		//    {
		//        Driver = rootDriver,
		//        DriverUID = rootDriver.UID
		//    };
		//    DeviceConfiguration.RootDevice = rootDevice;
		//    ushort descriptorNo = 0;
		//    int count = 0;
		//    while (true)
		//    {
		//        descriptorNo++;
		//        byte packNo = 1;
		//        var descriptorNoBytes = new List<byte>(BitConverter.GetBytes(descriptorNo));
		//        var data = new List<byte>(descriptorNoBytes);
		//        data.Add(packNo);
		//        var bytes = allbytes[count];
		//        count ++;
		//        if (bytes.Count < 5)
		//            break;
		//        if (bytes[3] == 0xff && bytes[4] == 0xff)
		//            break;
		//        Parce(bytes.Skip(3).ToList(), descriptorNo);
		//    }
		//    XManager.UpdateConfiguration();
		//}

		void Parce(List<byte> bytes, int descriptorNo)
		 {
			var internalType = BytesHelper.SubstructShort(bytes, 0);
			var controllerAdress = BytesHelper.SubstructShort(bytes, 2);
			var adressOnController = BytesHelper.SubstructShort(bytes, 4);
			//var physicalAdress = BytesHelper.SubstructShort(bytes, 6);
			if(internalType == 0)
				return;
			var letters = new List<byte>();
			for (int i = 8; i <= 39; i++)
			{
				byte letter = (byte)bytes[i];
				letters.Add(letter);
			}
			
			var memoryStream = new MemoryStream(letters.ToArray());
			var description = memoryStream.ToString();
			description = Encoding.GetEncoding(1251).GetString(letters.ToArray());
			description = description.TrimEnd();
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
					IntAddress = (byte)(adressOnController % 256),
					ShleifNo = (byte)(adressOnController / 256 + 1),
				};
				if (driver.DriverType == XDriverType.GK)
				{
					var ipAddressProperty = new XProperty()
					{
						Name = "IPAddress",
						StringValue = IpAddress
					};
					device.DeviceProperties.Add(ipAddressProperty);

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
			var descriptionParts = description.Split(new string[1] { " - " }, StringSplitOptions.None);
			if (descriptionParts.Count() == 2)
			{
				description = descriptionParts[0];
				no = (ushort)Int32.Parse(descriptionParts[1]);
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