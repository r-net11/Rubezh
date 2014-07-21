using System;
using System.Linq;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.GK;

namespace FiresecService
{
	public static class SKDProcessor
	{
		static SKDProcessor()
		{
#if DEBUG
			try
			{
				System.IO.File.Copy(@"..\..\..\ChinaController\CPPWrapper\Bin\CPPWrapper.dll", @"CPPWrapper.dll", true);
			}
			catch { }
#endif
		}

		public static void Start()
		{
			try
			{
				if (SKDManager.SKDConfiguration != null)
				{
					SKDManager.CreateDrivers();
					SKDManager.UpdateConfiguration();
				}
				ChinaSKDDriver.Processor.Start();
				foreach (var deviceProcessor in ChinaSKDDriver.Processor.DeviceProcessors)
				{
					deviceProcessor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
					deviceProcessor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);
				}

				ChinaSKDDriver.Processor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
				ChinaSKDDriver.Processor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);

				ChinaSKDDriver.Processor.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
				ChinaSKDDriver.Processor.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);

				ChinaSKDDriver.Processor.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
				ChinaSKDDriver.Processor.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDProcessor.Create");
			}
		}

		public static void Stop()
		{
			ChinaSKDDriver.Processor.Stop();
		}

		static void OnNewJournalItem(JournalItem journalItem)
		{
			journalItem.StateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
			FiresecService.Service.FiresecService.AddJournalItem(journalItem);
		}

		static void OnSKDCallbackResultEvent(SKDCallbackResult skdCallbackResult)
		{
			if (skdCallbackResult.SKDStates.DeviceStates.Count > 0)
			{
				foreach (var zone in SKDManager.Zones)
				{
					var stateClasses = GetZoneStateClasses(zone);

					var hasChanges = stateClasses.Count != zone.State.StateClasses.Count;
					if (!hasChanges)
					{
						foreach (var stateClass in stateClasses)
						{
							if (!zone.State.StateClasses.Contains(stateClass))
								hasChanges = true;
						}
					}

					if (hasChanges)
					{
						zone.State.StateClasses = stateClasses;
						zone.State.StateClass = zone.State.StateClasses.Min();
						skdCallbackResult.SKDStates.ZoneStates.Add(zone.State);
					}
				}
				foreach (var door in SKDManager.Doors)
				{
					var stateClasses = GetDoorStateClasses(door);

					var hasChanges = stateClasses.Count != door.State.StateClasses.Count;
					if (!hasChanges)
					{
						foreach (var stateClass in stateClasses)
						{
							if (!door.State.StateClasses.Contains(stateClass))
								hasChanges = true;
						}
					}

					if (hasChanges)
					{
						door.State.StateClasses = stateClasses;
						door.State.StateClass = door.State.StateClasses.Min();
						skdCallbackResult.SKDStates.DoorStates.Add(door.State);
					}
				}
			}

			FiresecService.Service.FiresecService.NotifySKDObjectStateChanged(skdCallbackResult);
		}

		static List<XStateClass> GetZoneStateClasses(SKDZone zone)
		{
			var stateClasses = new List<XStateClass>();
			foreach (var device in zone.Devices)
			{
				if (!stateClasses.Contains(device.State.StateClass))
					stateClasses.Add(device.State.StateClass);
			}
			stateClasses.Sort();
			if (stateClasses.Count == 0)
				stateClasses.Add(XStateClass.Norm);
			return stateClasses;
		}

		static List<XStateClass> GetDoorStateClasses(SKDDoor door)
		{
			var stateClasses = new List<XStateClass>();

			if (door.InDevice != null)
			{
				if (!stateClasses.Contains(door.InDevice.State.StateClass))
					stateClasses.Add(door.InDevice.State.StateClass);
			}
			if (door.OutDevice != null)
			{
				if (!stateClasses.Contains(door.OutDevice.State.StateClass))
					stateClasses.Add(door.OutDevice.State.StateClass);
			}

			stateClasses.Sort();
			if (stateClasses.Count == 0)
				stateClasses.Add(XStateClass.Norm);
			return stateClasses;
		}

		public static SKDStates SKDGetStates()
		{
			var skdStates = new SKDStates();
			foreach (var device in SKDManager.Devices)
			{
				skdStates.DeviceStates.Add(device.State);
			}
			foreach (var zone in SKDManager.Zones)
			{
				skdStates.ZoneStates.Add(zone.State);
			}
			return skdStates;
		}

		static void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifyGKProgress(gkProgressCallback);
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}
	}
}