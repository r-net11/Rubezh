using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static XDevice CopyDevice(XDevice device, bool fullCopy)
		{
			var newDevice = new XDevice()
			{
				DriverUID = device.DriverUID,
				Driver = device.Driver,
				ShleifNo = device.ShleifNo,
				IntAddress = device.IntAddress,
				Description = device.Description
			};

			if (fullCopy)
			{
				newDevice.UID = device.UID;
			}

			newDevice.Properties = new List<XProperty>();
			foreach (var property in device.Properties)
			{
				newDevice.Properties.Add(new XProperty()
				{
					Name = property.Name,
					Value = property.Value
				});
			}

			newDevice.Children = new List<XDevice>();
			foreach (var childDevice in device.Children)
			{
				var newChildDevice = CopyDevice(childDevice, fullCopy);
				newChildDevice.Parent = newDevice;
				newDevice.Children.Add(newChildDevice);
			}

			return newDevice;
		}

		public static XDevice AddChild(XDevice parentDevice, XDriver driver, byte shleifNo, byte intAddress)
		{
			var device = new XDevice()
			{
				DriverUID = driver.UID,
				Driver = driver,
				ShleifNo = shleifNo,
				IntAddress = intAddress,
				Parent = parentDevice
			};
			device.InitializeDefaultProperties();
			parentDevice.Children.Add(device);
			AddAutoCreateChildren(device);

			return device;
		}

		static void AddAutoCreateChildren(XDevice xDevice)
		{
			foreach (var autoCreateDriverType in xDevice.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
				for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
				{
					AddChild(xDevice, autoCreateDriver, 0, i);
				}
			}
			if (xDevice.Driver.DriverType == XDriverType.GK)
			{
				if (xDevice.Children.Count == 14)
				{
					xDevice.Children[0].Description = "Неисправность";
					xDevice.Children[1].Description = "Пожар 1";
					xDevice.Children[2].Description = "Пожар 2";
					xDevice.Children[3].Description = "Внимание";
					xDevice.Children[4].Description = "Включение СПТ";
					xDevice.Children[5].Description = "Тест";
					xDevice.Children[6].Description = "Отключение";
					xDevice.Children[7].Description = "Автоматика отключена";
					xDevice.Children[8].Description = "Звук отключен";
					xDevice.Children[9].Description = "Останов пуска";
					xDevice.Children[10].Description = "Линия 1";
					xDevice.Children[11].Description = "Линия 2";
					xDevice.Children[12].Description = "Реле 1";
					xDevice.Children[13].Description = "Реле 2";
				}
			}
		}

		public static void SynchronizeChildern(XDevice xDevice)
		{
			for (int i = xDevice.Children.Count(); i > 0; i--)
			{
				var childDevice = xDevice.Children[i - 1];

				if (xDevice.Driver.Children.Contains(childDevice.Driver.DriverType) == false)
				{
					xDevice.Children.RemoveAt(i - 1);
				}
			}

			foreach (var autoCreateDriverType in xDevice.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
				for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
				{
					var newDevice = new XDevice()
					{
						DriverUID = autoCreateDriver.UID,
						Driver = autoCreateDriver,
						IntAddress = i
					};
					if (xDevice.Children.Any(x => x.Driver.DriverType == newDevice.Driver.DriverType && x.Address == newDevice.Address) == false)
					{
						xDevice.Children.Add(newDevice);
						newDevice.Parent = xDevice;
					}
				}
			}
		}
	}
}
