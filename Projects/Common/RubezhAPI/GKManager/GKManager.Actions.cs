using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;

namespace RubezhAPI
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
			device.ChangedLogic();
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

		public static void AddDeviceToGuardZone(GKGuardZone guardZone, GKGuardZoneDevice guardZoneDevice)
		{
			var device = guardZoneDevice.Device;
			guardZone.GuardZoneDevices.Add(guardZoneDevice);
			if (!device.GuardZones.Contains(guardZone))
				device.GuardZones.Add(guardZone);
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
				device.GuardZones.RemoveAll(x => x.UID == guardZone.UID);
				guardZone.OutputDependentElements.RemoveAll(x => x.UID == device.UID);
				device.InputDependentElements.RemoveAll(x => x.UID == guardZone.UID);
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
		public static List<GKDevice> RemoveDevice(GKDevice device)
		{
			List<GKDevice> deviceAndChildren = new List<GKDevice>(device.AllChildrenAndSelf);
			foreach (var deviceItem in device.AllChildrenAndSelf)
			{
				deviceItem.Parent.Children.Remove(deviceItem);
				Devices.Remove(deviceItem);

				foreach (var zone in deviceItem.Zones)
				{
					zone.Devices.Remove(deviceItem);
					zone.OnChanged();
				}

				deviceItem.InputDependentElements.ForEach(x =>
				{
					x.OutputDependentElements.Remove(deviceItem);
					if (x is GKGuardZone)
						x.Invalidate(DeviceConfiguration);
				});

				deviceItem.OutputDependentElements.ForEach(x =>
				{
					x.InputDependentElements.Remove(deviceItem);
					x.Invalidate(DeviceConfiguration);
				});
			}
			return deviceAndChildren;
		}

		#region RebuildRSR2Addresses
		public static void RebuildRSR2Addresses(GKDevice parentDevice)
		{
			if (parentDevice.KAUShleifParent != null)
			{
				RebuildRSR2Addresses_Children = new List<GKDevice>();
				RebuildRSR2Addresses_AddChild(parentDevice.KAUShleifParent);

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
			else
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
					foreach (var device in mirrorParent.Children.Where(x => x.Driver.HasMirror))
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
		}

		static List<GKDevice> RebuildRSR2Addresses_Children;
		static void RebuildRSR2Addresses_AddChild(GKDevice device)
		{
			if (device.DriverType != GKDriverType.RSR2_MVP_Part && device.DriverType != GKDriverType.RSR2_KAU_Shleif && device.DriverType != GKDriverType.RSR2_KDKR_Part)
				RebuildRSR2Addresses_Children.Add(device);

			foreach (var child in device.Children)
			{
				RebuildRSR2Addresses_AddChild(child);
			}
		}
		#endregion

		public static GKDevice ChangeDriver(GKDevice device, GKDriver driver)
		{
			var count = device.AllChildren.Where(y => !y.Driver.IsGroupDevice && y.Driver.HasAddress).Count();
			if (!device.Driver.IsGroupDevice && device.Driver.HasAddress)
				count += 1;
			if ((GetAddress(device.KAUShleifParent.AllChildren) - count) + Math.Max(1, (int)driver.GroupDeviceChildrenCount) > 255)
				return null;

			var index = device.Parent.Children.IndexOf(device);
			RemoveDevice(device);
			return AddDevice(device.Parent, driver, 0, index);
		}

		public static int GetAddress(IEnumerable<GKDevice> children)
		{
			if (children.Count() > 0)
				return children.Max(x => x.IntAddress);
			else
				return 0;
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