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
        public static BinaryConfigurationHelper Current { get; set; }
		public List<BinaryPanel> BinaryPanels { get; set; }

		public BinaryConfigurationHelper()
		{
            Current = this;
			CreatePanels();
			CreateZones();
            CreateDirections();
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

		void AddLocalZoneToPanel(BinaryPanel binaryPanel, Zone zone)
		{
			var binaryZone = new BinaryZone(zone)
			{
				ParentPanel = binaryPanel.ParentPanel
			};
			if (!binaryPanel.LocalZones.Contains(zone))
			{
				binaryPanel.LocalZones.Add(zone);
				binaryPanel.BinaryLocalZones.Add(binaryZone);
			}
			if (!binaryZone.BinaryPanels.Contains(binaryPanel))
			{
				binaryZone.BinaryPanels.Add(binaryPanel);
			}
		}

		void AddRemoteZoneToPanel(BinaryPanel binaryPanel, Zone zone)
		{
			if (!binaryPanel.RemoteZones.Contains(zone))
			{
				var remoteBinaryPanels = new HashSet<Device>();
				foreach (var zoneDevice in zone.DevicesInZone)
				{
					var remoteBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == zoneDevice.ParentPanel);
					if (remoteBinaryPanel.ParentPanel.UID != binaryPanel.ParentPanel.UID)
					{
						remoteBinaryPanels.Add(remoteBinaryPanel.ParentPanel);
					}
				}

				foreach (var remoteBinaryPanel in remoteBinaryPanels)
				{
					var zoneBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == remoteBinaryPanel);
					var binaryZone = new BinaryZone(zone)
					{
						IsRemote = true,
						ParentPanel = remoteBinaryPanel
					};
					binaryZone.BinaryPanels.Add(binaryPanel);
					binaryZone.BinaryPanels.Add(zoneBinaryPanel);
					binaryPanel.RemoteZones.Add(zone);
					binaryPanel.BinaryRemoteZones.Add(binaryZone);
				}
			}
		}

		void CreateZones()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				var localBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == device.ParentPanel);
				if (device.Zone != null)
				{
					localBinaryPanel.TempLocalZones.Add(device.Zone);
				}
			}

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				var localBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == device.ParentPanel);

                var hasLocalZonesInAllClauses = false;
                foreach (var clause in device.ZoneLogic.Clauses)
                {
                    foreach (var zone in clause.Zones)
                    {
                        foreach (var deviceInZone in zone.DevicesInZone)
                        {
                            if (deviceInZone.ParentPanel.UID == localBinaryPanel.ParentPanel.UID)
                            {
                                hasLocalZonesInAllClauses = true;
                                break;
                            }
                        }
                    }
                }
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					var hasLocalZones = false;
					foreach (var zone in clause.Zones)
					{
						foreach (var deviceInZone in zone.DevicesInZone)
						{
							if(deviceInZone.ParentPanel.UID == localBinaryPanel.ParentPanel.UID)
							{
								hasLocalZones = true;
								break;
							}
						}
					}

					foreach (var zone in clause.Zones)
					{
						if (zone.DevicesInZone.Any(x => x.ParentPanel.UID == localBinaryPanel.ParentPanel.UID))
						{
							localBinaryPanel.TempLocalZones.Add(zone);
						}
                        if (clause.Operation.Value == ZoneLogicOperation.All)
						{
							localBinaryPanel.TempLocalZones.Add(zone);
							if (hasLocalZones)
							{
								localBinaryPanel.TempRemoteZones.Add(zone);
							}
						}
                        if (hasLocalZonesInAllClauses && device.ZoneLogic.JoinOperator == ZoneLogicJoinOperator.And)
                        {
                            localBinaryPanel.TempRemoteZones.Add(zone);
                        }
					}
				}
			}

			foreach (var binaryPanel in BinaryPanels)
			{
                foreach (var zone in binaryPanel.TempRemoteZones.OrderBy(x => x.No))
                {
                    AddRemoteZoneToPanel(binaryPanel, zone);
                }
				foreach (var zone in binaryPanel.TempLocalZones.OrderBy(x=>x.No))
				{
					AddLocalZoneToPanel(binaryPanel, zone);
				}
			}

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				var localBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == device.ParentPanel);

				foreach (var clause in device.ZoneLogic.Clauses)
				{
					var binaryPanels = new HashSet<Device>();
					foreach (var clauseZone in clause.Zones)
					{
						foreach (var zoneDevice in clauseZone.DevicesInZone)
						{
							var clauseBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == zoneDevice.ParentPanel);
							binaryPanels.Add(clauseBinaryPanel.ParentPanel);
						}
					}
					foreach (var zone in clause.Zones)
					{
						if (!device.BinaryDevice.BinaryZones.Any(x => x.Zone.UID == zone.UID))
						{
							var existingRemoteBinaryZone = localBinaryPanel.BinaryRemoteZones.FirstOrDefault(x => x.Zone.UID == zone.UID);
							if (existingRemoteBinaryZone != null)
							{
								existingRemoteBinaryZone.IsRemote = true;
								device.BinaryDevice.BinaryZones.Add(existingRemoteBinaryZone);
							}
							else
							{
								var existingLocalBinaryZone = localBinaryPanel.BinaryLocalZones.FirstOrDefault(x => x.Zone.UID == zone.UID);
								if (existingLocalBinaryZone != null)
								{
									device.BinaryDevice.BinaryZones.Add(existingLocalBinaryZone);
								}
							}
						}
					}
				}
			}

			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				var binaryPanels = new HashSet<Device>();
				foreach (var device in zone.DevicesInZone)
				{
					binaryPanels.Add(device.ParentPanel);
				}

				foreach (var binaryPanel in binaryPanels)
				{
					var parentPanel = binaryPanel;// zone.DevicesInZone.FirstOrDefault().ParentPanel;
					var localBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == parentPanel);

					foreach (var device in zone.DevicesInZoneLogic)
					{
						foreach (var clause in device.ZoneLogic.Clauses)
						{
							//if (clause.Operation.Value == ZoneLogicOperation.All)
							{
								foreach (var clauseZone in clause.Zones)
								{
									if (clauseZone.UID == zone.UID)
									{
										if (device.ParentPanel.UID != parentPanel.UID)
										{
											var binaryDevice = new BinaryDevice()
											{
												Device = device,
												ParentPanel = parentPanel,
											};
											localBinaryPanel.BinaryRemoteDevices.Add(binaryDevice);
										}
									}
								}
							}
						}
					}
				}
			}

			foreach (var binaryPanel in BinaryPanels)
			{
				binaryPanel.BinaryRemoteDevices = binaryPanel.BinaryRemoteDevices.OrderBy(x => x.Device.ParentPanel.IntAddress*256*256 + x.Device.IntAddress).ToList();
			}

			foreach (var binaryPanel in BinaryPanels)
			{
				binaryPanel.BinaryLocalZones = binaryPanel.BinaryLocalZones.OrderBy(x => x.Zone.No).ToList();
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
                foreach (var zoneUID in direction.ZoneUIDs)
                {
                    var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
                    if (zone != null)
                    {
                        foreach (var deviceInZone in zone.DevicesInZone)
                        {
                            var localBinaryPanel = BinaryPanels.FirstOrDefault(x => x.ParentPanel == deviceInZone.ParentPanel);
                            localBinaryPanel.TempDirections.Add(direction);
                        }
                    }
                }
			}

            foreach (var binaryPanel in BinaryPanels)
            {
                foreach (var direction in binaryPanel.TempDirections.OrderBy(x=>x.Id))
                {
                    var binaryDirection = new BinaryDirection()
                    {
                        Direction = direction
                    };
                    binaryPanel.BinaryDirections.Add(binaryDirection);
                }
            }
		}
	}
}