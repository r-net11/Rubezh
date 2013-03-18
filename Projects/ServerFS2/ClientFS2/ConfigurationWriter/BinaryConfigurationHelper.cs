using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI.Models.Binary;

namespace ClientFS2.ConfigurationWriter
{
	public class BinaryConfigurationHelper
	{
		public List<BinaryPanel> BinaryPanels { get; set; }

		public BinaryConfigurationHelper()
		{
			CreatePanels();
			CreateZones();

			foreach (var binaryPanel in BinaryPanels)
			{
				//binaryPanel.Initialize();
			}
		}

		void CreatePanels()
		{
			BinaryPanels = new List<BinaryPanel>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					var binaryPanel = new BinaryPanel(device);
					binaryPanel.CreatreDevices();
					BinaryPanels.Add(binaryPanel);
				}
			}
		}

		void CreateZones()
		{
			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				var binaryZone = new BinaryZone()
				{
					Zone = zone
				};
				var parantPanels = new HashSet<Device>();
				foreach (var device in zone.DevicesInZone)
				{
					parantPanels.Add(device.ParentPanel);
					var localBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == device.ParentPanel);
					binaryZone.ParentPanel = localBinaryPanel.ParentPanel;
					if (!localBinaryPanel.LocalZones.Contains(zone))
					{
						localBinaryPanel.LocalZones.Add(zone);
						localBinaryPanel.BinaryLocalZones.Add(binaryZone);
					}
					binaryZone.BinaryPanels.Add(localBinaryPanel);
				}
				foreach (var device in zone.DevicesInZoneLogic)
				{
					var binaryPanel = device.BinaryDevice.BinaryPanel;
					if (!binaryZone.BinaryPanels.Contains(binaryPanel))
					{
						if (!binaryPanel.RemoteZones.Contains(zone))
						{
							binaryPanel.RemoteZones.Add(zone);
							binaryPanel.BinaryRemoteZones.Add(binaryZone);
						}
					}

					foreach (var zoneBinaryPanel in binaryZone.BinaryPanels)
					{
						if (zoneBinaryPanel != binaryPanel)
						{
							if (!zoneBinaryPanel.remoteDevices.Contains(device))
							{
								zoneBinaryPanel.remoteDevices.Add(device);
								zoneBinaryPanel.BinaryRemoteDevices.Add(device.BinaryDevice);
							}
						}
					}

					device.BinaryDevice.BinaryZones.Add(binaryZone);

					//if (parantPanels.Contains(device.ParentPanel))
					//{
					//}
					//else
					//{
					//}
				}
			}
			foreach (var binaryPanel in BinaryPanels)
			{
				for (int i = 0; i < binaryPanel.BinaryLocalZones.Count; i++)
				{
					binaryPanel.BinaryLocalZones[i].LocalNo = i;
				}
			}
		}
	}
}