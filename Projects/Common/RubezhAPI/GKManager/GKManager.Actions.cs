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
			foreach (var deviceGuardZone in deviceGuardZones)
			{
				device.GuardZones.Add(deviceGuardZone.GuardZone);

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
			if (!zone.OutputDependentElements.Contains(device))
				zone.OutputDependentElements.Add(device);
			zone.Devices.Add(device);
			zone.OnChanged();
			device.OnChanged();
		}

		public static void AddDeviceToGuardZone(GKDevice device, GKGuardZone guardZone ,GKGuardZoneDevice guardZoneDevice = null)
		{
			if (guardZoneDevice == null)
				guardZoneDevice = new GKGuardZoneDevice();
			guardZone.GuardZoneDevices.Add(guardZoneDevice);
			if (!device.GuardZones.Contains(guardZone))
				device.GuardZones.Add(guardZone);
			if (!device.GuardZoneUIDs.Contains(guardZone.UID))
				device.GuardZoneUIDs.Add(guardZone.UID);
			if (!device.InputDependentElements.Contains(guardZone))
				device.InputDependentElements.Add(guardZone);
			if (!guardZone.OutputDependentElements.Contains(device))
				guardZone.OutputDependentElements.Add(device);
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
				zone.OutputDependentElements.Remove(device);
				device.InputDependentElements.Remove(zone);
				zone.OnChanged();
				device.OnChanged();
			}
		}

		public static void RemoveDeviceFromGuardZone(GKDevice device, GKGuardZone guardZone)
		{
			if (guardZone != null)
			{
				guardZone.GuardZoneDevices.RemoveAll(x => x.DeviceUID == device.UID);
				device.GuardZones.Remove(guardZone);
				device.OnChanged();
			}
		}

		public static void AddDevice(GKDevice device)
		{
			device.InitializeDefaultProperties();
		}

		/// <summary>
		/// Удаление устройства
		/// </summary>
		/// <param name="device"></param>
		public static void RemoveDevice(GKDevice device)
		{
			var parentDevice = device.Parent;
			foreach (var zone in device.Zones)
			{
				zone.Devices.Remove(device);
				zone.OnChanged();
			}

			parentDevice.Children.Remove(device);
			Devices.Remove(device);

			device.InputDependentElements.ForEach(x =>
			{
				x.OutputDependentElements.Remove(device);
				if (x is GKGuardZone)
					x.Invalidate(GKManager.DeviceConfiguration);
			});

			device.OutputDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(device);
				x.Invalidate(GKManager.DeviceConfiguration);
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

			foreach (var gkBase in device.OutputDependentElements)
			{
				gkBase.InputDependentElements.Remove(device);
				gkBase.OnChanged();
			}
			foreach (var gkBase in device.InputDependentElements)
			{
				gkBase.OutputDependentElements.Remove(device);
				gkBase.OnChanged();
			}
			if (device.Children != null)
			{
				device.Children.ForEach(x =>
				{
					GKManager.Devices.Remove(x);
					x.OutputDependentElements.ForEach(y =>
					{
						y.InputDependentElements.Remove(x);
						y.ChangedLogic();
						y.OnChanged();
					});
					x.InputDependentElements.ForEach(y =>
					{
						y.OutputDependentElements.Remove(x);
						if (y is GKGuardZone)
						{
							y.Invalidate(GKManager.DeviceConfiguration);
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
				SetDeviceLogic (device, new GKLogic());
			}

			device.Properties = new List<GKProperty>();
			Guid oldUID = device.UID;
			device.UID = Guid.NewGuid();

			device.OnUIDChanged(oldUID, device.UID);

			device.OutputDependentElements.ForEach(x =>
			{
				x.Invalidate(GKManager.DeviceConfiguration);
				x.OnChanged();
			});

			device.InputDependentElements.ForEach(x =>
			{
				if (x is GKGuardZone)
				{
					x.Invalidate(GKManager.DeviceConfiguration);
					x.OnChanged();
				}
				x.UpdateLogic(GKManager.DeviceConfiguration);
				x.OnChanged();
			});
			device.Zones = new List<GKZone>();
			device.ZoneUIDs = new List<Guid>();
			device.GuardZones = new List<GKGuardZone>();
			device.InputDependentElements = new List<GKBase>();
			device.OutputDependentElements = new List<GKBase>();
			device.IsInMPT = false;
		
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

		public static void ClearMirror(GKDevice mirrorDevice)
		{
			mirrorDevice.GKReflectionItem.Devices = new List<GKDevice>();
			mirrorDevice.GKReflectionItem.DeviceUIDs = new List<Guid>();
			mirrorDevice.GKReflectionItem.Zones = new List<GKZone>();
			mirrorDevice.GKReflectionItem.ZoneUIDs = new List<Guid>();
			mirrorDevice.GKReflectionItem.GuardZones = new List<GKGuardZone>();
			mirrorDevice.GKReflectionItem.GuardZoneUIDs = new List<Guid>();
			mirrorDevice.GKReflectionItem.Diretions = new List<GKDirection>();
			mirrorDevice.GKReflectionItem.DiretionUIDs = new List<Guid>();
			mirrorDevice.GKReflectionItem.Delays = new List<GKDelay>();
			mirrorDevice.GKReflectionItem.DelayUIDs = new List<Guid>();
			mirrorDevice.GKReflectionItem.NSs = new List<GKPumpStation>();
			mirrorDevice.GKReflectionItem.NSUIDs = new List<Guid>();
			mirrorDevice.GKReflectionItem.MPTs = new List<GKMPT>();
			mirrorDevice.GKReflectionItem.MPTUIDs = new List<Guid>();
		}

		public static void AddToMirror(GKBase gkBase, GKDevice mirrorDevice)
		{
			if (gkBase is GKDevice)
			{
				mirrorDevice.GKReflectionItem.Devices.Add(gkBase as GKDevice);
				mirrorDevice.GKReflectionItem.DeviceUIDs.Add(gkBase.UID);
			}

			if (gkBase is GKZone)
			{
				mirrorDevice.GKReflectionItem.Zones.Add(gkBase as GKZone);
				mirrorDevice.GKReflectionItem.ZoneUIDs.Add(gkBase.UID);
			}

			if (gkBase is GKGuardZone)
			{
				mirrorDevice.GKReflectionItem.GuardZones.Add(gkBase as GKGuardZone);
				mirrorDevice.GKReflectionItem.GuardZoneUIDs.Add(gkBase.UID);
			}

			if (gkBase is GKDirection)
			{
				mirrorDevice.GKReflectionItem.Diretions.Add(gkBase as GKDirection);
				mirrorDevice.GKReflectionItem.DiretionUIDs.Add(gkBase.UID);
			}

			if (gkBase is GKDelay)
			{
				mirrorDevice.GKReflectionItem.Delays.Add(gkBase as GKDelay);
				mirrorDevice.GKReflectionItem.DelayUIDs.Add(gkBase.UID);
			}

			if (gkBase is GKPumpStation)
			{
				mirrorDevice.GKReflectionItem.NSs.Add(gkBase as GKPumpStation);
				mirrorDevice.GKReflectionItem.NSUIDs.Add(gkBase.UID);
			}

			if (gkBase is GKMPT)
			{
				mirrorDevice.GKReflectionItem.MPTs.Add(gkBase as GKMPT);
				mirrorDevice.GKReflectionItem.MPTUIDs.Add(gkBase.UID);
			}

			//mirrorDevice.AddDependentElement(gkBase);
			//if (mirrorDevice.DriverType != GKDriverType.DetectorDevicesMirror)
			//	gkBase.AddDependentElement(mirrorDevice);
		}
	}
}