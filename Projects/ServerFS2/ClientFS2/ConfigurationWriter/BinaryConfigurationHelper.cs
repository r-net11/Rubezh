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

					foreach (var clause in device.ZoneLogic.Clauses)
					{
						var binaryPanels = new HashSet<Device>();
						foreach (var clauseZone in clause.Zones)
						{
							var clauseBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == device.ParentPanel);
							binaryPanels.Add(clauseBinaryPanel.ParentPanel);
						}
						if (!binaryZone.BinaryPanels.Contains(binaryPanel))
						{
							binaryZone.BinaryPanels.Add(binaryPanel);
						}
						if (!binaryPanel.LocalZones.Contains(zone))
						{
							binaryPanel.LocalZones.Add(zone);
							binaryPanel.BinaryLocalZones.Add(binaryZone);
						}
						if (!binaryPanel.LocalZones.Contains(zone))
						{
							//if (binaryPanels.Count > 1)
							{
								//if (clause.Operation.Value == ZoneLogicOperation.All)
								{
									if (!binaryPanel.RemoteZones.Contains(zone))
									{
										binaryPanel.RemoteZones.Add(zone);
										binaryPanel.BinaryRemoteZones.Add(binaryZone);
									}
								}
							}
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
				}
			}

			foreach (var binaryPanel in BinaryPanels)
			{
				for (int i = 0; i < binaryPanel.BinaryLocalZones.Count; i++)
				{
					binaryPanel.BinaryLocalZones[i].LocalNo = i+1;
				}
			}
		}

		
		void CreateDirections()
		{
			foreach (var direction in ConfigurationManager.DeviceConfiguration.Directions)
			{
				var binaryDirection = new BinaryDirection()
				{
					Direction = direction
				};
			}
		}
	}
}