using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;

namespace RubezhClient
{
	public partial class GKManager
	{
		public static void ChangeDeviceZones(GKDevice device, List<GKZone> zones)
		{
			foreach (var zone in device.Zones)
			{
				zone.Devices.Remove(device);
				zone.OnChanged();
			}
			device.Zones.Clear();
			device.ZoneUIDs.Clear();
			foreach (var zone in zones)
			{
				device.Zones.Add(zone);
				device.ZoneUIDs.Add(zone.UID);
				zone.Devices.Add(device);
				zone.OnChanged();
			}
			device.OnChanged();
		}

		public static void ChangeDeviceGuardZones(GKDevice device, List<GKDeviceGuardZone> deviceGuardZones)
		{
			foreach (var guardZone in device.GuardZones)
			{
				guardZone.GuardZoneDevices.RemoveAll(x => x.Device == device);
				guardZone.OnChanged();
			}
			device.GuardZones.Clear();
			device.GuardZoneUIDs.Clear();
			foreach (var deviceGuardZone in deviceGuardZones)
			{
				device.GuardZones.Add(deviceGuardZone.GuardZone);
				device.GuardZoneUIDs.Add(deviceGuardZone.GuardZoneUID);

				var gkGuardZoneDevice = new GKGuardZoneDevice();
				gkGuardZoneDevice.Device = device;
				gkGuardZoneDevice.DeviceUID = device.UID;
				if (deviceGuardZone.ActionType != null)
					gkGuardZoneDevice.ActionType = deviceGuardZone.ActionType.Value;
				gkGuardZoneDevice.CodeReaderSettings = deviceGuardZone.CodeReaderSettings;
				deviceGuardZone.GuardZone.GuardZoneDevices.Add(gkGuardZoneDevice);
				deviceGuardZone.GuardZone.OnChanged();
			}
			device.OnChanged();
		}

		public static void AddDeviceToZone(GKDevice device, GKZone zone)
		{
			if (!device.Zones.Contains(zone))
			{
				device.Zones.Add(zone);
			}
			if (!device.ZoneUIDs.Contains(zone.UID))
				device.ZoneUIDs.Add(zone.UID);
			if (!device.InputDependentElements.Contains(zone))
				device.InputDependentElements.Add(zone);
			if (!zone.OutDependentElements.Contains(device))
				zone.OutDependentElements.Add(device);
			zone.Devices.Add(device);
			zone.OnChanged();
			device.OnChanged();
		}

		public static void AddDeviceToGuardZone(GKDevice device, GKGuardZone guardZone)
		{
			if (!device.GuardZones.Contains(guardZone))
			{
				device.GuardZones.Add(guardZone);
			}
			if (!device.GuardZoneUIDs.Contains(guardZone.UID))
				device.GuardZoneUIDs.Add(guardZone.UID);
			if (!device.InputDependentElements.Contains(guardZone))
				device.InputDependentElements.Add(guardZone);
			if (!guardZone.OutDependentElements.Contains(device))
				guardZone.OutDependentElements.Add(device);
			guardZone.OnChanged();
			device.OnChanged();
		}

		public static void RemoveDeviceFromZone(GKDevice device, GKZone zone)
		{
			if (zone != null)
			{
				device.Zones.Remove(zone);
				device.ZoneUIDs.Remove(zone.UID);
				zone.Devices.Remove(device);
				zone.OnChanged();
				device.OnChanged();
			}
		}

		public static void RemoveDeviceFromGuardZone(GKDevice device, GKGuardZone guardZone)
		{
			if (guardZone != null)
			{
				device.GuardZones.Remove(guardZone);
				device.GuardZoneUIDs.Remove(guardZone.UID);
				device.OnChanged();
			}
		}

		public static void AddDevice(GKDevice device)
		{
			device.InitializeDefaultProperties();
		}

		public static void RemoveDevice(GKDevice device)
		{
			var parentDevice = device.Parent;
			foreach (var zone in device.Zones)
			{
				zone.Devices.Remove(device);
				zone.OnChanged();
			}

			var deviceMPTs = MPTs.FindAll(x => x.MPTDevices.Any(y => y.DeviceUID == device.UID));
			foreach (var deviceMPT in deviceMPTs)
			{
				foreach (var mptDevice in deviceMPT.MPTDevices.FindAll(x => x.DeviceUID == device.UID))
				{
					mptDevice.Device = null;
					mptDevice.DeviceUID = Guid.Empty;
				}
			}
			parentDevice.Children.Remove(device);
			Devices.Remove(device);

			device.InputDependentElements.ForEach(x =>
			{
				x.OutDependentElements.Remove(device);
				if (x is GKGuardZone)
					x.Invalidate();
			});

			device.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(device);
				x.UpdateLogic();
				x.OnChanged();
			});
		}

		#region RebuildRSR2Addresses
		public static void RebuildRSR2Addresses(GKDevice parentDevice)
		{
			var kauParent = parentDevice.KAUParent;
			if (kauParent != null)
			{
				foreach (var shliefDevice in kauParent.Children)
				{
					RebuildRSR2Addresses_Children = new List<GKDevice>();
					RebuildRSR2Addresses_AddChild(shliefDevice);

					byte currentAddress = 1;
					foreach (var device in RebuildRSR2Addresses_Children)
					{
						device.IntAddress = currentAddress;
						if (!device.Driver.IsGroupDevice)
						{
							currentAddress++;
						}
						device.OnChanged();
					}

					RebuildRSR2Addresses_Children.FindAll(x => x.Driver.IsGroupDevice).ForEach(x => x.OnChanged());
				}
			}

			var mirrorParent = parentDevice.MirrorParent;
			if (mirrorParent != null)
			{
				int currentAddress = 1;
				foreach (var device in mirrorParent.Children)
				{
					device.IntAddress = currentAddress;
					if (!device.Driver.IsGroupDevice)
					{
						currentAddress++;
					}
					device.OnChanged();
				}

			}

		}

		static List<GKDevice> RebuildRSR2Addresses_Children;
		static void RebuildRSR2Addresses_AddChild(GKDevice device)
		{
			if (device.DriverType != GKDriverType.RSR2_MVP_Part && device.DriverType != GKDriverType.RSR2_KAU_Shleif)
				RebuildRSR2Addresses_Children.Add(device);

			foreach (var child in device.Children)
			{
				RebuildRSR2Addresses_AddChild(child);
			}
		}
		#endregion

		public static void AddZone(GKZone zone)
		{
			Zones.Add(zone);
		}

		public static void RemoveZone(GKZone zone)
		{
			Zones.Remove(zone);
			zone.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(zone);
				if (x is GKDevice)
				{
					x.Invalidate();
					x.OnChanged();
				}
				    x.UpdateLogic();
					x.OnChanged();
			});

			foreach (var device in zone.Devices)
			{
				device.Zones.Remove(zone);
				device.ZoneUIDs.Remove(zone.UID);
				device.OnChanged();
			}
				
			zone.OnChanged();
		}

		public static void EditZone(GKZone zone)
		{
			zone.OnChanged();
			zone.OutDependentElements.ForEach(x => x.OnChanged());
		}

		/// <summary>
		/// Adds specified Delay.
		/// </summary>
		/// <param name="delay">Delay to add.</param>
		public static void AddDelay(GKDelay delay)
		{
			Delays.Add(delay);
		}

		/// <summary>
		/// Removes specified Delay.
		/// </summary>
		/// <param name="delay">Delay to remove.</param>
		public static void RemoveDelay(GKDelay delay)
		{
			throw new NotImplementedException();
		}

		public static void AddDirection(GKDirection direction)
		{
			Directions.Add(direction);
		}

		public static void RemoveDirection(GKDirection direction)
		{
			Directions.Remove(direction);
			direction.InputDependentElements.ForEach(x =>
			{
				x.OutDependentElements.Remove(direction);
				x.OnChanged();
			});

			direction.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(direction);
				x.OnChanged();
			});
			direction.OnChanged();
		}

		public static void ChangeLogic(GKDevice device, GKLogic logic)
		{

			device.Logic = logic;
			DeviceConfiguration.InvalidateOneLogic(device, device.Logic);
			device.OnChanged();
		}

		public static bool ChangeDriver(GKDevice device, GKDriver driver)
		{
			var kauShleifParent = device.KAUShleifParent;
			if (kauShleifParent != null)
			{
				var maxAddress = 0;
				if (kauShleifParent.Children.Count > 0)
				{
					maxAddress = kauShleifParent.Children.Max(x => x.IntAddress);
				}
				if (maxAddress + (driver.GroupDeviceChildrenCount > 0 ? driver.GroupDeviceChildrenCount : 1) - 1 > 255)
				{
					return false;
				}
			}

			device.OutDependentElements.ForEach(x => x.InputDependentElements.Remove(device));
			device.InputDependentElements.ForEach(x => x.OutDependentElements.Remove(device));
			if (device.Children != null)
			{
				device.Children.ForEach(x =>
				{
					GKManager.Devices.Remove(x);
					x.OutDependentElements.ForEach(y =>
					{
						y.InputDependentElements.Remove(x);
						y.ChangedLogic();
						y.OnChanged();
					});
					x.InputDependentElements.ForEach(y =>
					{
						y.OutDependentElements.Remove(x);
						if (y is GKGuardZone)
						{
							y.Invalidate();
						}
						if (y is GKZone)
						{
							GKManager.Zones.ForEach(zone =>
							{
								if (zone == y)
									zone.Devices.Remove(x);
							});
						}
					});
				});
			}

			var changeZone = !(device.Driver.HasZone && driver.HasLogic);
			device.Driver = driver;
			device.DriverUID = driver.UID;
			if (driver.IsRangeEnabled)
				device.IntAddress = driver.MinAddress;

			device.Children.Clear();
			AddAutoCreateChildren(device);
			if (driver.IsGroupDevice)
			{
				var groupDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

				for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
				{
					var autoDevice = GKManager.AddChild(device, null, groupDriver, (byte)(device.IntAddress + i));
				}
			}

			if (changeZone)
			{
				RemoveDeviceFromZone(device, null);
				device.Zones.ForEach(x => x.Devices.Remove(device));
				ChangeLogic(device, new GKLogic());
			}

			device.Properties = new List<GKProperty>();
			device.UID = Guid.NewGuid();
			device.OutDependentElements.ForEach(x =>
			{
				x.UpdateLogic();
				x.OnChanged();
			});

			device.InputDependentElements.ForEach(x =>
			{
				if (x is GKGuardZone)
				{
					x.Invalidate();
					x.OnChanged();
				}
				x.UpdateLogic();
				x.OnChanged();
			});
			device.Zones = new List<GKZone>();
			device.ZoneUIDs = new List<Guid>();
			device.GuardZones = new List<GKGuardZone>();
			device.GuardZoneUIDs = new List<Guid>();
			device.InputDependentElements = new List<GKBase>();
			device.OutDependentElements = new List<GKBase>();
		
			return true;
		}

		public static void RemoveSKDZone(GKSKDZone zone)
		{
			SKDZones.Remove(zone);
			zone.OnChanged();
		}

		public static void EditSKDZone(GKSKDZone zone)
		{
			zone.OnChanged();
		}
	}
}