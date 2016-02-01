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
			for (int i = 0; i < e.Count; i++)
			{
				if (e.ItemIds[i].TagId == 0)
					continue;

				var tag = TagDevices.FirstOrDefault(x => x.TagId == e.ItemIds[i].TagId);
				
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

					var device = GKManager.Devices.FirstOrDefault(x => x.UID == tag.UID);

					if (device != null)
					{
						ExecuteCmdForDevice(device, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}

					var zone = GKManager.Zones.FirstOrDefault(x => x.UID == tag.UID);

					if (zone != null)
					{
						ExecuteCmdForZone(zone, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;						
					}

					var direction = GKManager.Directions.FirstOrDefault(x => x.UID == tag.UID);

					if (direction != null)
					{
						ExecuteCmdForDirection(direction, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}

					var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == tag.UID);

					if (guardZone != null)
					{
						ExecuteCmdForGuardZone(guardZone, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}

					var delay = GKManager.Delays.FirstOrDefault(x => x.UID == tag.UID);

					if (delay != null)
					{
						ExecuteCmdForDelay(delay, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}

					var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == tag.UID);

					if (mpt != null)
					{
						ExecuteCmdForMPT(mpt, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}

					var pump = GKManager.PumpStations.FirstOrDefault(x => x.UID == tag.UID);

					if (pump != null)
					{
						ExecuteCmdForPumpStation(pump, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}

					var door = GKManager.Doors.FirstOrDefault(x => x.UID == tag.UID);

					if (door != null)
					{
						ExecuteCmdForDoor(door, cmd);
						e.Errors[i] = ErrorCodes.False;
						e.ItemIds[i].TagId = 0;
						e.MasterError = ErrorCodes.False;
						continue;
					}

					// Необходимо, что бы значение не было записано в тег,
					// а приходило по обратной связи после выполения команды
					//throw new CancelWritingException();
					e.Errors[i] = ErrorCodes.False;
					e.ItemIds[i].TagId = 0;
					e.MasterError = ErrorCodes.False;
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

		private static void ExecuteCmdForDevice(GKDevice device, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(device);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(device);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(device);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(device);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(device);
						break;
					}
				case Commands.TurnOnNow: 
					{
						ClientManager.FiresecService.GKTurnOnNow(device);
						break; 
					}
				case Commands.TurnOffNow: 
					{
						ClientManager.FiresecService.GKTurnOffNow(device);
						break; 
					}
				case Commands.Stop: 
					{
						ClientManager.FiresecService.GKStop(device);
						break; 
					}
				case Commands.Reset: 
					{
						ClientManager.FiresecService.GKReset(device);
						break; 
					}
			}
		}
		private static void ExecuteCmdForZone(GKZone zone, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(zone);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(zone);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(zone);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(zone);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(zone);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(zone);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(zone);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(zone);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(zone);
						break;
					}
			}
		}
		private static void ExecuteCmdForDirection(GKDirection direction, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(direction);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(direction);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(direction);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(direction);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(direction);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(direction);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(direction);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(direction);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(direction);
						break;
					}
			}
		}
		private static void ExecuteCmdForGuardZone(GKGuardZone zone, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(zone);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(zone);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(zone);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(zone);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(zone);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(zone);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(zone);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(zone);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(zone);
						break;
					}
			}
		}
		private static void ExecuteCmdForDelay(GKDelay delay, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(delay);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(delay);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(delay);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(delay);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(delay);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(delay);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(delay);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(delay);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(delay);
						break;
					}
			}
		}
		private static void ExecuteCmdForMPT(GKMPT mpt, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(mpt);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(mpt);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(mpt);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(mpt);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(mpt);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(mpt);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(mpt);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(mpt);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(mpt);
						break;
					}
			}
		}
		private static void ExecuteCmdForPumpStation(GKPumpStation pumpStation, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(pumpStation);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(pumpStation);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(pumpStation);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(pumpStation);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(pumpStation);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(pumpStation);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(pumpStation);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(pumpStation);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(pumpStation);
						break;
					}
			}
		}
		private static void ExecuteCmdForDoor(GKDoor door, Commands cmd)
		{
			switch (cmd)
			{
				case Commands.SetAutomaticMode:
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(door);
						break;
					}
				case Commands.SetManualMode:
					{
						ClientManager.FiresecService.GKSetManualRegime(door);
						break;
					}
				case Commands.SetDisabledMode:
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(door);
						break;
					}
				case Commands.TurnOff:
					{
						ClientManager.FiresecService.GKTurnOff(door);
						break;
					}
				case Commands.TurnOn:
					{
						ClientManager.FiresecService.GKTurnOn(door);
						break;
					}
				case Commands.TurnOnNow:
					{
						ClientManager.FiresecService.GKTurnOnNow(door);
						break;
					}
				case Commands.TurnOffNow:
					{
						ClientManager.FiresecService.GKTurnOffNow(door);
						break;
					}
				case Commands.Stop:
					{
						ClientManager.FiresecService.GKStop(door);
						break;
					}
				case Commands.Reset:
					{
						ClientManager.FiresecService.GKReset(door);
						break;
					}
			}
		}
	}
}