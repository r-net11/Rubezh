using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule.Converter
{
	public class FiresecToGKConverter
	{
		XDevice gkDevice;

		public void Convert()
		{
			PrepareKAU();

			var convertationViewModel = new ConvertationViewModel();
			if (DialogService.ShowModalWindow(convertationViewModel))
			{
				foreach (var fsShleif in convertationViewModel.FSShleifs)
				{
					foreach (var childDevice in fsShleif.FSDevice.Children)
					{
						if (childDevice.IntAddress / 256 == fsShleif.FSShleifNo)
							AddDevice(childDevice, fsShleif.KAUDevice, (byte)fsShleif.KAUShleifNo);
					}
				}
				ConvertZones();
				ConvertLogic();
				XManager.UpdateConfiguration();
			}
		}

		void PrepareKAU()
		{
			AddRootDevices();
			var totalShleifCount = 0;
			foreach (var panelDevice in GetPanels())
			{
				totalShleifCount += panelDevice.Driver.ShleifCount;
			}
			var kauCount = (totalShleifCount + 1) / 8 + Math.Min(1, totalShleifCount % 8);
			for (int i = 0; i < kauCount; i++)
			{
				XManager.AddChild(gkDevice, XManager.Drivers.FirstOrDefault(x => x.IsKauOrRSR2Kau), (byte)(i + 1));
			}
			XManager.UpdateConfiguration();
		}

		public void AddRootDevices()
		{
			XManager.DeviceConfiguration = new XDeviceConfiguration();

			var systemDevice = new XDevice()
			{
				BaseUID = Guid.NewGuid(),
				DriverUID = XDriver.System_UID,
				Driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
			};
			XManager.DeviceConfiguration.Devices.Add(systemDevice);
			XManager.DeviceConfiguration.RootDevice = systemDevice;

			var gkDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.GK);
			gkDevice = XManager.AddChild(systemDevice, gkDriver, 1);
			gkDevice.BaseUID = Guid.NewGuid();
			XManager.DeviceConfiguration.Devices.Add(gkDevice);
		}

		IEnumerable<Device> GetPanels()
		{
			foreach (var device in FiresecManager.Devices)
			{
				if (device.Driver.IsPanel)
					yield return device;
			}
		}

		public void AddDevice(Device fsDevice, XDevice kauDevice, byte shleifNo)
		{
			var driver = XManager.Drivers.FirstOrDefault(x => x.UID == fsDevice.DriverUID);
			if (driver == null)
			{
				return;
			}

			var device = new XDevice()
			{
				BaseUID = fsDevice.UID,
				DriverUID = driver.UID,
				Driver = driver,
				IntAddress = (byte)(fsDevice.IntAddress & 0xff),
				Description = fsDevice.Description
			};
			XManager.DeviceConfiguration.Devices.Add(device);
			var shleifDevice = kauDevice.Children.FirstOrDefault(x => x.ShleifNo == shleifNo);
			if (shleifDevice != null)
			{
				shleifDevice.Children.Add(device);
				device.Parent = shleifDevice;
			}

			foreach (var fsChildDevice in fsDevice.Children)
			{
				AddDevice(fsChildDevice, device, shleifNo);
			}
		}

		void ConvertZones()
		{
			foreach (var zone in FiresecManager.Zones)
			{
				var xZone = new XZone()
				{
					BaseUID = zone.UID,
					No = (ushort)zone.No,
					Name = zone.Name,
					Description = zone.Description,
					Fire1Count = (ushort)zone.DetectorCount,
				};
				XManager.Zones.Add(xZone);
			}

			foreach (var device in FiresecManager.Devices)
			{
				var xDevice = XManager.Devices.FirstOrDefault(x => x.BaseUID == device.UID);
				if (xDevice != null)
				{
					if ((device.Driver.IsZoneDevice) && (device.ZoneUID != Guid.Empty) && (device.Driver.DriverType != DriverType.MPT))
					{
						var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
						var xZone = XManager.Zones.FirstOrDefault(x => x.No == (ushort)zone.No);
						if (zone != null)
						{
							xDevice.ZoneUIDs.Add(xZone.BaseUID);
						}
					}
				}
			}
		}

		void ConvertLogic()
		{
			foreach (var xDevice in XManager.Devices)
			{
				var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == xDevice.BaseUID);
				if (device != null)
				{
					if ((device.Driver.IsZoneLogicDevice) && (device.ZoneLogic != null))
					{
						var xDeviceLogic = new XDeviceLogic();

						foreach (var clause in device.ZoneLogic.Clauses)
						{
							var xClause = new XClause()
							{
								ClauseOperationType = ClauseOperationType.AllZones
							};
							if (clause.Operation.HasValue)
								switch (clause.Operation.Value)
								{
									case ZoneLogicOperation.All:
										xClause.ClauseOperationType = ClauseOperationType.AllZones;
										break;

									case ZoneLogicOperation.Any:
										xClause.ClauseOperationType = ClauseOperationType.AnyZone;
										break;
								}
							switch (clause.State)
							{
								case FiresecAPI.Models.ZoneLogicState.Attention:
									xClause.StateType = XStateBit.Attention;
									break;

								case FiresecAPI.Models.ZoneLogicState.Fire:
									xClause.StateType = XStateBit.Fire1;
									break;

								case FiresecAPI.Models.ZoneLogicState.Failure:
									xClause.StateType = XStateBit.Failure;
									break;

								default:
									continue;
							}
							if ((clause.ZoneUIDs == null) || (clause.ZoneUIDs.Count == 0))
								continue;

							foreach (var zoneUID in clause.ZoneUIDs)
							{
								var xZone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
								xClause.ZoneUIDs.Add(xZone.BaseUID);
							}

							xDeviceLogic.Clauses.Add(xClause);
						}

						if (xDeviceLogic.Clauses.Count > 0)
							xDevice.DeviceLogic = xDeviceLogic;
					}
					if (device.Driver.DriverType == DriverType.MPT)
					{
						if (device.Zone != null)
						{
							var xClause = new XClause();
							xClause.ClauseOperationType = ClauseOperationType.AnyZone;
							xClause.ZoneUIDs.Add(device.Zone.UID);
							xDevice.DeviceLogic.Clauses.Add(xClause);
						}
					}
				}
			}
		}
	}
}