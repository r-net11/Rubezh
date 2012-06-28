using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graybox.OPC.ServerToolkit.CLRWrapper;
using System.Threading;
using FiresecService.Service;
using Common;
using FiresecAPI.Models;

namespace FiresecService.OPC
{
	public static class FiresecOPCManager
	{
		static Guid srvGuid = new Guid("FBBF742D-3077-40F4-9877-C97F5EE4CE0E");
		static Thread thread;
		static OPCDAServer srv = new OPCDAServer();
		static List<TagDevice> TagDevices = new List<TagDevice>();
		static List<TagZone> TagZones = new List<TagZone>();
		static int TagsCount = 0;

		public static void Start()
		{
			thread = new Thread(new ThreadStart(OnRun));
			thread.SetApartmentState(ApartmentState.MTA);
			thread.Start();
		}

		public static void Stop()
		{
			thread.Abort();
			if (srv != null)
			{
				srv.RevokeClassObject();
			}
		}

		public static void OPCRefresh(DeviceConfiguration deviceConfiguration)
		{
			ConfigurationCash.DeviceConfiguration = deviceConfiguration;
			Stop();
			Start();
		}

		public static void OPCRegister()
		{
			OPCDAServer.RegisterServer(
				srvGuid,
				"Rubezh",
				"FiresecOPC",
				"Rubezh.FiresecOPC",
				"1.0");
		}

		public static void OPCUnRegister()
		{
			OPCDAServer.UnregisterServer(srvGuid);
		}

		static void OnRun()
		{
			try
			{
				Run();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecOPCManager.OnRun");
			}
		}

		static void Run()
		{
			srv.Events.ServerReleased += new ServerReleasedEventHandler(Events_ServerReleased);
			srv.Events.ReadItems += new ReadItemsEventHandler(Events_ReadItems);
			srv.Events.WriteItems += new WriteItemsEventHandler(Events_WriteItems);
			srv.Events.ActivateItems += new ActivateItemsEventHandler(Events_ActivateItems);
			srv.Events.DeactivateItems += new DeactivateItemsEventHandler(Events_DeactivateItems);

			srv.Initialize(srvGuid, 50, 50, ServerOptions.NoAccessPaths, '/', 100);

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				if (TagsCount >= 100)
					break;

				if (!device.IsOPCUsed)
					continue;

				var tagName = new StringBuilder();
				foreach (var parentDevice in device.AllParents)
				{
					if (parentDevice.Driver.HasAddress)
						tagName.Append(parentDevice.Driver.ShortName + " - " + parentDevice.PresentationAddress + "/");
					else
						tagName.Append(parentDevice.Driver.ShortName + "/");
				}
				if (device.Driver.HasAddress)
					tagName.Append(device.Driver.ShortName + " - " + device.PresentationAddress + "/");
				else
					tagName.Append(device.Driver.ShortName + "/");

				var tagId = srv.CreateTag(
					TagsCount,
					tagName.ToString(),
					AccessRights.readable,
					(double)0);

				var tagDevice = new TagDevice()
				{
					TagId = tagId,
					DeviceState = ClientsCash.MonitoringFiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID)
				};
				TagDevices.Add(tagDevice);
				TagsCount++;
			}
			foreach (var zone in ConfigurationCash.DeviceConfiguration.Zones)
			{
				if (TagsCount >= 100)
					break;

				if (!zone.IsOPCUsed)
					continue;

				var tagId = srv.CreateTag(
					TagsCount,
					"Zones/" + zone.Name,
					AccessRights.readable,
					(double)0);

				var tagZone = new TagZone()
				{
					TagId = tagId,
					ZoneState = ClientsCash.MonitoringFiresecManager.DeviceConfigurationStates.ZoneStates.FirstOrDefault(x => x.No == zone.No)
				};
				TagZones.Add(tagZone);
				TagsCount++;
			}

			srv.RegisterClassObject();

			srv.BeginUpdate();
			foreach (var tagDevice in TagDevices)
			{
				srv.SetTag(tagDevice.TagId, tagDevice.DeviceState.StateType);
			}
			foreach (var tagZone in TagZones)
			{
				srv.SetTag(tagZone.TagId, tagZone.ZoneState.StateType);
			}
			srv.EndUpdate(false);

			ClientsCash.MonitoringFiresecManager.Watcher.DevicesStateChanged += new Action<List<DeviceState>>(OnDevicesStateChanged);
			ClientsCash.MonitoringFiresecManager.Watcher.ZoneStateChanged += new Action<ZoneState>(OnZoneStateChanged);

			while (true)
			{
				Thread.Sleep(500);
			}
		}

		static void OnDevicesStateChanged(List<DeviceState> deviceStates)
		{
			if (deviceStates == null)
			{
				Logger.Error("FiresecOPCManager.OnDevicesStateChanged deviceStates = null");
				return;
			}

			srv.BeginUpdate();
			foreach (var deviceState in deviceStates)
			{
				var tagDevice = TagDevices.FirstOrDefault(x => x.DeviceState.UID == deviceState.UID);
				if (tagDevice == null)
				{
					Logger.Error("FiresecOPCManager.OnDevicesStateChanged tagDevice = null");
					continue;
				}
				if (tagDevice.DeviceState == null)
				{
					Logger.Error("FiresecOPCManager.OnDevicesStateChanged tagDevice.DeviceState = null");
					continue;
				}
				srv.SetTag(tagDevice.TagId, tagDevice.DeviceState.StateType);
			}
			srv.EndUpdate(false);
		}

		static void OnZoneStateChanged(ZoneState zoneState)
		{
			if (zoneState == null)
			{
				Logger.Error("FiresecOPCManager.OnZoneStateChanged zoneState = null");
				return;
			}

			srv.BeginUpdate();
			var tagZone = TagZones.FirstOrDefault(x => x.ZoneState.No == zoneState.No);
			if (tagZone == null)
			{
				Logger.Error("FiresecOPCManager.OnZoneStateChanged tagZone = null");
				return;
			}
			if (tagZone.ZoneState == null)
			{
				Logger.Error("FiresecOPCManager.OnZoneStateChanged tagZone.ZoneState = null");
				return;
			}
			srv.SetTag(tagZone.TagId, tagZone.ZoneState.StateType);
			srv.EndUpdate(false);
		}

		static void Events_DeactivateItems(object sender, DeactivateItemsArgs e)
		{
		}

		static void Events_ActivateItems(object sender, ActivateItemsArgs e)
		{
		}

		static void Events_WriteItems(object sender, WriteItemsArgs e)
		{
		}

		static void Events_ReadItems(object sender, ReadItemsArgs e)
		{
		}

		static void Events_ServerReleased(object sender, ServerReleasedArgs e)
		{
		}
	}
}