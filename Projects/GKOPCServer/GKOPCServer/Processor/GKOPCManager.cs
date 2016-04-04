using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using RubezhAPI.GK;
using RubezhClient;
using Graybox.OPC.ServerToolkit.CLRWrapper;
using RubezhAPI;
using System.Runtime.InteropServices;

namespace GKOPCServer
{
	public static class GKOPCManager
	{
		static Guid srvGuid = new Guid("C8F9CDB2-5BD7-4369-8DEC-4514CE236DF5");
		static Thread thread;
		public static OPCDAServer OPCDAServer { get; private set; }
		static List<TagBase> TagDevices = new List<TagBase>();
		static List<TagBase> TagZones = new List<TagBase>();
		static List<TagBase> TagDirections = new List<TagBase>();
		static List<TagBase> TagGuardZones = new List<TagBase>();
		static List<TagBase> TagDelays = new List<TagBase>();
		static List<TagBase> TagMPTs = new List<TagBase>();
		static List<TagBase> TagPumpStations = new List<TagBase>();
		static List<TagBase> TagDoors = new List<TagBase>();
		static int TagsCount = 0;
		static AutoResetEvent StopEvent;

		public static void Start()
		{
			//OPCRegister();
			try
			{
				StopEvent = new AutoResetEvent(false);
				thread = new Thread(new ThreadStart(OnRun));
				thread.Name = "GKOPCManager";
				thread.SetApartmentState(ApartmentState.MTA);
				thread.Start();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове GKOPCManager.Start");
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
#if DEBUG
				MessageBox.Show(e.ToString());
#endif
				Logger.Error(e, "Исключение при вызове GKOPCManager.OnRun");
			}
		}

		static void Run()
		{
			OPCDAServer = new OPCDAServer();
			OPCDAServer.Events.ServerReleased += new ServerReleasedEventHandler(Events_ServerReleased);
			OPCDAServer.Events.ReadItems += new ReadItemsEventHandler(Events_ReadItems);
			OPCDAServer.Events.WriteItems += new WriteItemsEventHandler(Events_WriteItems);
			OPCDAServer.Events.ActivateItems += new ActivateItemsEventHandler(Events_ActivateItems);
			OPCDAServer.Events.DeactivateItems += new DeactivateItemsEventHandler(Events_DeactivateItems);

			OPCDAServer.Initialize(srvGuid, 50, 50, ServerOptions.NoAccessPaths, '/', 100);

			foreach (var device in GKManager.Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DeviceUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;
				var tagName = new StringBuilder();
				foreach (var parentDevice in device.AllParents)
				{
					if (parentDevice.Driver.DriverType != GKDriverType.System)
					{
						if (parentDevice.Driver.HasAddress)
							tagName.Append(parentDevice.ShortName + " - " + parentDevice.PresentationAddress + "/");
						else
							tagName.Append(parentDevice.ShortName + "/");
					}
				}
				if (device.Driver.HasAddress)
					tagName.Append(device.ShortName + " - " + device.PresentationAddress + "/");
				else
					tagName.Append(device.ShortName + "/");

				var name = tagName.ToString();
				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					name,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagDevice = new TagBase(tagId, device.State, device.UID);
				TagDevices.Add(tagDevice);
				TagsCount++;
			}

			foreach (var zone in GKManager.Zones.Where(x => GKManager.DeviceConfiguration.OPCSettings.ZoneUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"Zones/" + zone.PresentationName,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagZone = new TagBase(tagId, zone.State, zone.UID);
				TagZones.Add(tagZone);
				TagsCount++;
			}

			foreach (var direction in GKManager.Directions.Where(x=> GKManager.DeviceConfiguration.OPCSettings.DiretionUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"Directions/" + direction.PresentationName,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagDirection = new TagBase(tagId, direction.State, direction.UID);
				TagDirections.Add(tagDirection);
				TagsCount++;
			}

			foreach (var zone in GKManager.GuardZones.Where(x => GKManager.DeviceConfiguration.OPCSettings.GuardZoneUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"GuardZones/" + zone.PresentationName,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagGuardZone = new TagBase(tagId, zone.State, zone.UID);
				TagGuardZones.Add(tagGuardZone);
				TagsCount++;
			}

			foreach (var delay in GKManager.Delays.Where(x => GKManager.DeviceConfiguration.OPCSettings.DelayUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"Delays/" + delay.PresentationName,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagdelay = new TagBase(tagId, delay.State, delay.UID);
				TagDelays.Add(tagdelay);
				TagsCount++;
			}

			foreach (var mpt in GKManager.MPTs.Where(x => GKManager.DeviceConfiguration.OPCSettings.MPTUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"MPTs/" + mpt.PresentationName,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagmpt = new TagBase(tagId, mpt.State, mpt.UID);
				TagMPTs.Add(tagmpt);
				TagsCount++;
			}

			foreach (var pump in GKManager.PumpStations.Where(x => GKManager.DeviceConfiguration.OPCSettings.NSUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"PumpStations/" + pump.PresentationName,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagns = new TagBase(tagId, pump.State, pump.UID);
				TagPumpStations.Add(tagns);
				TagsCount++;
			}

			foreach (var door in GKManager.Doors.Where(x => GKManager.DeviceConfiguration.OPCSettings.DoorUIDs.Contains(x.UID)).ToList())
			{
				if (TagsCount >= 100)
					break;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"Doors/" + door.PresentationName,
					//AccessRights.readable,
					AccessRights.readWritable,
					(double)0);

				var tagdoor = new TagBase(tagId, door.State, door.UID);
				TagDoors.Add(tagdoor);
				TagsCount++;
			}

			OPCDAServer.RegisterClassObject();

			OPCDAServer.BeginUpdate();
			foreach (var tagDevice in TagDevices)
			{
				OPCDAServer.SetTag(tagDevice.TagId, tagDevice.State.StateClass);
			}
			foreach (var tagZone in TagZones)
			{
				OPCDAServer.SetTag(tagZone.TagId, tagZone.State.StateClass);
			}
			foreach (var tagDirection in TagDirections)
			{
				OPCDAServer.SetTag(tagDirection.TagId, tagDirection.State.StateClass);
			}
			foreach (var tagGuardZone in TagGuardZones)
			{
				OPCDAServer.SetTag(tagGuardZone.TagId, tagGuardZone.State.StateClass);	
			}
			foreach (var tagDelay in TagDelays)
			{
				OPCDAServer.SetTag(tagDelay.TagId, tagDelay.State.StateClass);
			}
			foreach (var tagMPT in TagMPTs)
			{
				OPCDAServer.SetTag(tagMPT.TagId, tagMPT.State.StateClass);
			}
			foreach (var tagPumps in TagPumpStations)
			{
				OPCDAServer.SetTag(tagPumps.TagId, tagPumps.State.StateClass);
			}
			foreach (var tagDoors in TagDoors)
			{
				OPCDAServer.SetTag(tagDoors.TagId, tagDoors.State.StateClass);
			}
			OPCDAServer.EndUpdate(false);

			while (true)
			{
				if (StopEvent.WaitOne(5000))
					break;
			}
			OPCDAServer.RevokeClassObject();
			OPCDAServer = null;
		}

		static void Events_DeactivateItems(object sender, DeactivateItemsArgs e)
		{
		}

		static void Events_ActivateItems(object sender, ActivateItemsArgs e)
		{
		}

		static void Events_WriteItems(object sender, WriteItemsArgs e)
		{
			GKBase gkBase;

			for (int i = 0; i < e.Count; i++)
			{
				if (e.ItemIds[i].TagId == 0)
					continue;

				var tag = Tags.FirstOrDefault(t => t.TagId == e.ItemIds[i].TagId);

				if (tag == null)
				{
					e.Errors[i] = ErrorCodes.False;
					e.ItemIds[i].TagId = 0;
					e.MasterError = ErrorCodes.False;
					continue;
				}

				try
				{
					var stateCode = Convert.ToInt32(e.Values[i]);
					var cmd = (Commands)stateCode;

					gkBase = GKObjects.FirstOrDefault(x => x.UID == tag.UID);

					if (gkBase == null)
					{
						// Необходимо, что бы значение не было записано в тег,
						// а приходило по обратной связи после выполения команды
						// throw new CancelWritingException();
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}
					else
					{
						ExecuteCmd(gkBase, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
					}
				}
				catch (Exception ex)
				{
					e.Errors[i] = (ErrorCodes)Marshal.GetHRForException(ex);
					e.ItemIds[i].TagId = 0;
					e.MasterError = ErrorCodes.False;
				}
			}
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
				"GKFiresecOPC",
				"Rubezh.GKFiresecOPC",
				"1.0");
		}

		public static void OPCUnRegister()
		{
			OPCDAServer.UnregisterServer(srvGuid);
		}

		static void ExecuteCmd(GKBase gkBase, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(gkBase);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(gkBase);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(gkBase);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(gkBase);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(gkBase);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(gkBase);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(gkBase);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(gkBase);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(gkBase);
						break;
					}
			}
		}

		static TagBase[] Tags
		{
			get
			{
				List<TagBase> allTags = new List<TagBase>();
				allTags.AddRange(TagDevices);
				allTags.AddRange(TagZones);
				allTags.AddRange(TagDirections);
				allTags.AddRange(TagGuardZones);
				allTags.AddRange(TagDelays);
				allTags.AddRange(TagMPTs);
				allTags.AddRange(TagPumpStations);
				allTags.AddRange(TagDoors);
				return allTags.ToArray();
			}
		}

		static GKBase[] GKObjects
		{
			get
			{
				List<GKBase> list = new List<GKBase>();
				list.AddRange(GKManager.Devices);
				list.AddRange(GKManager.Zones);
				list.AddRange(GKManager.Directions);
				list.AddRange(GKManager.GuardZones);
				list.AddRange(GKManager.Delays);
				list.AddRange(GKManager.MPTs);
				list.AddRange(GKManager.PumpStations);
				list.AddRange(GKManager.Doors);
				return list.ToArray();
			}
		}
	}
}