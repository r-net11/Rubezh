using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.Diagnostics
{
	public class GKToFiresecConverter
	{
		public void Convert()
		{
			KauDevices = XManager.Devices.Where(x => x.DriverType == XDriverType.KAU);
			PanelDevices = new List<Device>();
			CreatePaneDevices();
			foreach (var kauDevice in KauDevices)
			{
				AddDevice(kauDevice);
			}
			FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
		}

		List<Device> PanelDevices;
		IEnumerable<XDevice> KauDevices;

		void CreatePaneDevices()
		{
			var deviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration = new DeviceConfiguration();
			deviceConfiguration.RootDevice = new Device()
			{
				Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer)
			};
			deviceConfiguration.RootDevice.DriverUID = deviceConfiguration.RootDevice.Driver.UID;
			var msDevice = new Device()
			{
				Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_1)
			};
			msDevice.DriverUID = msDevice.Driver.UID;
			deviceConfiguration.RootDevice.Children.Add(msDevice);
			var channelDevice = new Device()
			{
				Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.USB_Channel_1),
				IntAddress = 1
			};
			channelDevice.DriverUID = channelDevice.Driver.UID;
			msDevice.Children.Add(channelDevice);

			for (int i = 0; i < KauDevices.Count() * 4; i++)
			{
				var panelDevice = new Device()
				{
					Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM),
					IntAddress = i + 1
				};
				panelDevice.DriverUID = panelDevice.Driver.UID;
				channelDevice.Children.Add(panelDevice);
				PanelDevices.Add(panelDevice);
			}
		}

		void AddDevice(XDevice parentXDevice, Device parentDevice = null)
		{
			foreach (var xDevice in parentXDevice.Children)
			{
				var device = new Device();
				device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == xDevice.Driver.UID);
				if (device.Driver == null)
					continue;
				device.DriverUID = device.Driver.UID;
				device.IntAddress = ((xDevice.ShleifNoNew - 1) % 2 + 1) * 256 + xDevice.IntAddress;
				if (parentDevice == null)
				{
					var panelDevice = PanelDevices[(parentXDevice.IntAddress - 1) * 4 + (xDevice.ShleifNoNew - 1) / 2];
					panelDevice.Children.Add(device);
				}
				else
				{
					parentDevice.Children.Add(device);
				}
				AddDevice(xDevice, device);
			}
		}
	}
}