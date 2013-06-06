using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static Device AutoDetectDevice()
		{
			byte deviceCount;
			var bytes = new List<byte>();
			bytes.Add(0x01);
			bytes.Add(0x01);
			bytes.Add(0x04);
			var res = SendCode(bytes);

			var computerDevice = new Device();
			var msDevice = new Device();
			var usbChannel1Device = new Device();
			var usbChannel2Device = new Device();

			// Добавляем компьютер
			computerDevice.DriverUID = new Guid("F8340ECE-C950-498D-88CD-DCBABBC604F3");
			computerDevice.Driver = Drivers.FirstOrDefault(x => x.UID == computerDevice.DriverUID);

			// МС-1
			byte ms = 0x03;
			msDevice.DriverUID = new Guid("FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6");
			msDevice.Driver = Drivers.FirstOrDefault(x => x.UID == msDevice.DriverUID);

			// Добавляем 1-й канал
			usbChannel1Device.DriverUID = new Guid("780DE2E6-8EDD-4CFA-8320-E832EB699544");
			usbChannel1Device.Driver = Drivers.FirstOrDefault(x => x.UID == usbChannel1Device.DriverUID);
			usbChannel1Device.IntAddress = 1;
			msDevice.Children.Add(usbChannel1Device);

			if (res[5] == 0x41) // запрашиваем второй шлейф
			{
				// МС-2
				ms = 0x04;
				msDevice.DriverUID = new Guid("CD0E9AA0-FD60-48B8-B8D7-F496448FADE6");
				msDevice.Driver = Drivers.FirstOrDefault(x => x.UID == msDevice.DriverUID);

				// Добавляем 2-й канал
				usbChannel2Device.DriverUID = new Guid("F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E");
				usbChannel2Device.Driver = Drivers.FirstOrDefault(x => x.UID == usbChannel2Device.DriverUID);
				usbChannel2Device.IntAddress = 2;
				msDevice.Children.Add(usbChannel2Device);
			}

			// Добавляем МС
			computerDevice.Children.Add(msDevice);
			for (byte sleif = 0x03; sleif <= ms; sleif++)
			{
				for (deviceCount = 1; deviceCount < 128; deviceCount++)
				{
					if (Progress != null)
						Progress(Convert.ToInt32(deviceCount), 127, (sleif - 2) + " - Канал. Поиск PNP-устройств Рубеж с адресом: " + deviceCount + ". Всего адресов: 127");
					bytes = new List<byte>();
					bytes.Add(sleif);
					bytes.Add(deviceCount);
					bytes.Add(0x3C);
					var inputBytes = SendCode(bytes, 5000, 500);
					if (inputBytes[6] == 0x7C) // Если по данному адресу найдено устройство, узнаем тип устройства и его версию ПО
					{
						var device = new Device();
						device.Properties = new List<Property>();
						device.Driver = new Driver();
						device.IntAddress = inputBytes[5];
						device.Driver.HasAddress = true;
						bytes = new List<byte>();
						bytes.Add(sleif);
						bytes.Add(deviceCount);
						bytes.Add(0x01);
						bytes.Add(0x03);
						inputBytes = SendCode(bytes);
						device.Driver = Drivers.FirstOrDefault(x => x.UID == DriversHelper.GetDriverUidByType(inputBytes[7]));

						bytes = new List<byte>();
						bytes.Add(sleif);
						bytes.Add(deviceCount);
						bytes.Add(0x01);
						bytes.Add(0x12);
						inputBytes = SendCode(bytes);
						device.Properties.Add(new Property() { Name = "Version", Value = inputBytes[7].ToString("X2") + "." + inputBytes[8].ToString("X2") });

						bytes = new List<byte>();
						bytes.Add(sleif);
						bytes.Add(deviceCount);
						bytes.Add(0x01);
						bytes.Add(0x52);
						bytes.Add(0x00);
						bytes.Add(0x00);
						bytes.Add(0x00);
						bytes.Add(0xF4);
						bytes.Add(0x0B);
						inputBytes = SendCode(bytes);
						if (inputBytes.Count >= 18)
						{
							var serilaNo = "";
							for (int i = 7; i <= 18; i++)
								serilaNo += inputBytes[i] - 0x30 + ".";
							serilaNo = serilaNo.Remove(serilaNo.Length - 1);
							device.Properties.Add(new Property() { Name = "SerialNo", Value = serilaNo });
						}
						if (sleif == 0x03)
							usbChannel1Device.Children.Add(device);
						else
							usbChannel2Device.Children.Add(device);
					}
				}
			}
			return computerDevice;
		}
	}
}