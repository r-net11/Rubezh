using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Threading;
using Graybox.OPC.ServerToolkit.CLRWrapper;
using FiresecAPI.Models;
using FiresecClient;
using System.Diagnostics;

namespace FiresecOPCServer
{
	public static class FiresecOPCManager
	{
		static Guid srvGuid = new Guid("FBBF742D-3077-40F4-9877-C97F5EE4CE0E");
		static Thread thread;
		static OPCDAServer srv;
		static List<TagDevice> TagDevices = new List<TagDevice>();
		static List<TagZone> TagZones = new List<TagZone>();
		static int TagsCount = 0;
		static AutoResetEvent StopEvent;

		public static void Start()
		{
			OPCRegister();
			try
			{
				StopEvent = new AutoResetEvent(false);
				thread = new Thread(new ThreadStart(OnRun));
				thread.SetApartmentState(ApartmentState.MTA);
				thread.Start();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecOPCManager.Start");
			}
		}

		public static void Stop()
		{
			StopEvent.Set();
		}

		public static void OPCRefresh()
		{
			Stop();
			Start();
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
			srv = new OPCDAServer();
			srv.Events.ServerReleased += new ServerReleasedEventHandler(Events_ServerReleased);
			srv.Events.ReadItems += new ReadItemsEventHandler(Events_ReadItems);
			srv.Events.WriteItems += new WriteItemsEventHandler(Events_WriteItems);
			srv.Events.ActivateItems += new ActivateItemsEventHandler(Events_ActivateItems);
			srv.Events.DeactivateItems += new DeactivateItemsEventHandler(Events_DeactivateItems);

			srv.Initialize(srvGuid, 50, 50, ServerOptions.NoAccessPaths, '/', 100);

			if (FiresecManager.Devices == null)
				return;

			foreach (var device in FiresecManager.Devices)
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
					DeviceState = device.DeviceState
				};
				TagDevices.Add(tagDevice);
				TagsCount++;
			}
			foreach (var zone in (from Zone zone in FiresecManager.Zones orderby zone.No select zone))
			{
				if (TagsCount >= 100)
					break;

				if (!zone.IsOPCUsed)
					continue;

				var tagId = srv.CreateTag(
					TagsCount,
					"Zones/" + zone.PresentationName,
					AccessRights.readable,
					(double)0);

				var tagZone = new TagZone()
				{
					TagId = tagId,
					ZoneState = zone.ZoneState
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

			FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>(OnDevicesStateChanged);
			FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>(OnZoneStatesChanged);

			while (true)
			{
				if (StopEvent.WaitOne(5000))
					break;
			}
			srv.RevokeClassObject();
			srv = null;
		}

		static void OnDevicesStateChanged(List<DeviceState> deviceStates)
		{
			if (deviceStates == null)
			{
				Logger.Error("FiresecOPCManager.OnDevicesStateChanged deviceStates = null");
				return;
			}
			if (srv == null)
			{
				Logger.Error("FiresecOPCManager.OnDevicesStateChanged srv = null");
				return;
			}

			srv.BeginUpdate();
			foreach (var deviceState in deviceStates)
			{
				var tagDevice = TagDevices.FirstOrDefault(x => x.DeviceState.Device.UID == deviceState.Device.UID);
				if (tagDevice == null)
				{
					continue;
				}
				if (tagDevice.DeviceState == null)
				{
					Logger.Error("FiresecOPCManager.OnDevicesStateChanged tagDevice.DeviceState = null");
					continue;
				}
				srv.UpdateTags(new int[1] { tagDevice.TagId }, new object[1] { tagDevice.DeviceState.StateType }, Quality.Good, FileTime.UtcNow, ErrorCodes.Ok, false);
			}
			srv.EndUpdate(false);
		}

		static void OnZoneStatesChanged(List<ZoneState> zoneStates)
		{
			if (zoneStates == null)
			{
				Logger.Error("FiresecOPCManager.OnZoneStateChanged zoneState = null");
				return;
			}
			if (srv == null)
			{
				Logger.Error("FiresecOPCManager.OnDevicesStateChanged srv = null");
				return;
			}

			srv.BeginUpdate();
			foreach (var zoneState in zoneStates)
			{
				var tagZone = TagZones.FirstOrDefault(x => x.ZoneState.Zone.No == zoneState.Zone.No);
				if (tagZone == null)
				{
					continue;
				}
				if (tagZone.ZoneState == null)
				{
					Logger.Error("FiresecOPCManager.OnZoneStatesChanged tagZone.ZoneState = null");
					continue;
				}
				srv.UpdateTags(new int[1] { tagZone.TagId }, new object[1] { tagZone.ZoneState.StateType }, Quality.Good, FileTime.UtcNow, ErrorCodes.Ok, false);
			}
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
			e.Suspend = false;
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
	}
}