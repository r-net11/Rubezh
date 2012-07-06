using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecService.Database;

namespace FiresecService.Processor
{
	public class Watcher
	{
		public bool DoNotCallback = false;

		public event Action<List<DeviceState>> DevicesStateChanged;
		void OnDevicesStateChanged(List<DeviceState> deviceStates)
		{
			foreach (var firesecService in FiresecServices)
			{
				if (firesecService != null && firesecService.CallbackWrapper != null)
				{
					firesecService.CallbackWrapper.DeviceStateChanged(ChangedDevices.ToList());
				}
			}

			if (DevicesStateChanged != null)
				DevicesStateChanged(deviceStates);
		}

		public event Action<List<DeviceState>> DevicesParametersChanged;
		void OnDevicesParametersChanged(List<DeviceState> deviceStates)
		{
			foreach (var firesecService in FiresecServices)
			{
				if (firesecService != null && firesecService.CallbackWrapper != null)
				{
					firesecService.CallbackWrapper.DeviceParametersChanged(ChangedDevices.ToList());
				}
			}

			if (DevicesParametersChanged != null)
				DevicesParametersChanged(deviceStates);
		}

		public event Action<ZoneState> ZoneStateChanged;
		void OnZoneStateChanged(ZoneState zoneState)
		{
			foreach (var firesecService in FiresecServices)
			{
				if (firesecService != null && firesecService.CallbackWrapper != null)
				{
					firesecService.CallbackWrapper.ZoneStateChanged(zoneState);
				}
			}

			if (ZoneStateChanged != null)
				ZoneStateChanged(zoneState);
		}

		FiresecManager FiresecManager;
		List<FiresecService.Service.FiresecService> FiresecServices
		{
			get { return FiresecManager.FiresecServices; }
		}
		FiresecSerializedClient FiresecSerializedClient
		{
			get { return FiresecManager.FiresecSerializedClient; }
		}

		public Watcher(FiresecManager firesecManager)
		{
			FiresecManager = firesecManager;

			if (firesecManager.MustMonitorStates)
			{
				SynchrinizeJournal();
				SetLastEvent();
				FiresecSerializedClient.NewEvent += new Action<int>(FiresecClient_NewEvent);
				OnStateChanged();
				OnParametersChanged();
			}
			FiresecSerializedClient.Progress += new Func<int, string, int, int, bool>(FiresecInternalClient_Progress);
		}

		bool FiresecInternalClient_Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			foreach (var firesecService in FiresecServices)
			{
				if (firesecService != null && firesecService.CallbackWrapper != null)
				{
					return firesecService.CallbackWrapper.Progress(stage, comment, percentComplete, bytesRW);
				}
			}
			return false;
		}

		void FiresecClient_NewEvent(int EventMask)
		{
			bool evmNewEvents = ((EventMask & 1) == 1);
			bool evmStateChanged = ((EventMask & 2) == 2);
			bool evmConfigChanged = ((EventMask & 4) == 4);
			bool evmDeviceParamsUpdated = ((EventMask & 8) == 8);
			bool evmPong = ((EventMask & 16) == 16);
			bool evmDatabaseChanged = ((EventMask & 32) == 32);
			bool evmReportsChanged = ((EventMask & 64) == 64);
			bool evmSoundsChanged = ((EventMask & 128) == 128);
			bool evmLibraryChanged = ((EventMask & 256) == 256);
			bool evmPing = ((EventMask & 512) == 512);
			bool evmIgnoreListChanged = ((EventMask & 1024) == 1024);
			bool evmEventViewChanged = ((EventMask & 2048) == 2048);

			if (evmStateChanged)
				OnStateChanged();

			if (evmDeviceParamsUpdated)
				OnParametersChanged();

			if (evmNewEvents)
				OnNewEvent();
		}

		int LastEventId = 0;
		HashSet<DeviceState> ChangedDevices;

		void SetLastEvent()
		{
			Firesec.Journals.document journal = FiresecSerializedClient.ReadEvents(0, 100).Result;
			if (journal != null && journal.Journal.IsNotNullOrEmpty())
			{
				foreach (var journalItem in journal.Journal)
				{
					var intValue = int.Parse(journalItem.IDEvents);
					if (intValue > LastEventId)
						LastEventId = intValue;
				}
			}
		}

		void SynchrinizeJournal()
		{
			var lastId = DatabaseHelper.GetLastOldId();
			if (lastId >= 0)
			{
				var journalRecords = GetEventsFromLastId(LastEventId);
				foreach (var journalRecord in journalRecords)
				{
					DatabaseHelper.AddJournalRecord(journalRecord);
				}
			}
		}

		List<JournalRecord> GetEventsFromLastId(int lastId)
		{
			var result = new List<JournalRecord>();

			var hasNewRecords = true;
			while (hasNewRecords)
			{
				hasNewRecords = false;
				var document = FiresecSerializedClient.ReadEvents(LastEventId, 100).Result;
				if (document != null && document.Journal.IsNotNullOrEmpty())
				{
					foreach (var innerJournalItem in document.Journal)
					{
						var eventId = int.Parse(innerJournalItem.IDEvents);
						if (eventId > LastEventId)
						{
							LastEventId = eventId;
							var journalRecord = JournalConverter.Convert(innerJournalItem);
							result.Add(journalRecord);
							hasNewRecords = true;
						}
					}
				}
			}

			return result;
		}

		void OnNewEvent()
		{
			var journalRecords = GetEventsFromLastId(LastEventId);
			foreach (var journalRecord in journalRecords)
			{
				var isNewEvent = DatabaseHelper.AddJournalRecord(journalRecord);
				if (isNewEvent)
				{
					foreach (var firesecService in FiresecServices)
					{
						if (firesecService != null && firesecService.CallbackWrapper != null)
						{
							firesecService.CallbackWrapper.NewJournalRecord(journalRecord);
						}
					}
				}
			}
		}

		public void OnParametersChanged()
		{
			ChangedDevices = new HashSet<DeviceState>();
			var coreParameters = FiresecSerializedClient.GetDeviceParams().Result;
			try
			{
				foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
				{
					var innerDevice = coreParameters.dev.FirstOrDefault(x => x.name == deviceState.PlaceInTree);
					if (innerDevice != null)
					{
						foreach (var parameter in deviceState.Parameters)
						{
							if (innerDevice.dev_param != null &&
								innerDevice.dev_param.Any(x => x.name == parameter.Name))
							{
								var innerParameter = innerDevice.dev_param.FirstOrDefault(x => x.name == parameter.Name);
								if (parameter.Value != innerParameter.value)
								{
									ChangedDevices.Add(deviceState);
								}
								parameter.Value = innerParameter.value;
							}
						}
					}
				}

				if (ChangedDevices.Count > 0)
				{
					OnDevicesParametersChanged(ChangedDevices.ToList());
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Watcher.OnParametersChanged");
			}
		}

		public void OnStateChanged()
		{
			try
			{
				ChangedDevices = new HashSet<DeviceState>();
				var coreState = FiresecSerializedClient.GetCoreState().Result;

				SetStates(coreState);
				PropogateStatesDown();
				PropogateStatesUp();
				CalculateZones();

				if (ChangedDevices.Count > 0)
				{
					OnDevicesStateChanged(ChangedDevices.ToList());
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Watcher.OnStateChanged");
			}
		}

		void SetStates(Firesec.CoreState.config coreState)
		{
			if (FiresecManager.DeviceConfigurationStates == null)
			{
				Logger.Error("Watcher.SetStates FiresecManager.DeviceConfigurationStates = null");
				return;
			}
			if (FiresecManager.DeviceConfigurationStates.DeviceStates == null)
			{
				Logger.Error("Watcher.SetStates FiresecManager.DeviceConfigurationStates.DeviceStates = null");
				return;
			}

			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				if (deviceState == null)
				{
					Logger.Error("Watcher.SetStates deviceState = null");
					return;
				}
				if (coreState == null)
				{
					Logger.Error("Watcher.SetStates coreState = null");
					return;
				}
				if (coreState.dev == null)
				{
					Logger.Error("Watcher.SetStates coreState.dev = null");
					return;
				}
				if (deviceState.PlaceInTree == null)
				{
					Logger.Error("Watcher.SetStates deviceState.PlaceInTree = null");
					return;
				}

				bool hasOneChangedState = false;

				Firesec.CoreState.devType innerDevice = FindDevice(coreState.dev, deviceState.PlaceInTree);
				if (innerDevice != null)
				{
					if (deviceState.Device == null)
					{
						Logger.Error("Watcher.SetStates deviceState.Device = null");
						return;
					}
					if (deviceState.Device.Driver == null)
					{
						Logger.Error("Watcher.SetStates deviceState.Device.Driver = null");
						return;
					}
					if (deviceState.Device.Driver.States == null)
					{
						Logger.Error("Watcher.SetStates deviceState.Device.Driver.States = null");
						return;
					}
					foreach (var driverState in deviceState.Device.Driver.States)
					{
						var innerState = innerDevice.state.FirstOrDefault(a => a.id == driverState.Id);
						if (deviceState.States == null)
						{
							Logger.Error("Watcher.SetStates deviceState.States = null");
							return;
						}
						var state = deviceState.States.FirstOrDefault(x => x.Code == driverState.Code);
						if ((state != null) != (innerState != null))
						{
							hasOneChangedState = true;
						}

						if (innerState != null)
						{
							if (state == null)
							{
								state = new DeviceDriverState()
								{
									Code = driverState.Code,
									DriverState = driverState.Copy()
								};
								deviceState.States.Add(state);
							}

							if (innerState.time != null)
								state.Time = JournalConverter.ConvertTime(innerState.time);
							else
								state.Time = null;
						}
						else
						{
							if (state != null)
								deviceState.States.Remove(state);
						}
					}
				}
				else
				{
					hasOneChangedState = deviceState.States.Count > 0;
					deviceState.States.Clear();
				}

				if (hasOneChangedState)
				{
					ChangedDevices.Add(deviceState);
				}
			}
		}

		void PropogateStatesDown()
		{
			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				deviceState.ParentStates.ForEach(x => x.IsDeleting = true);
			}

			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				foreach (var state in deviceState.States.Where(x => x.DriverState.AffectChildren))
				{
					foreach (var chilDevice in FiresecManager.DeviceConfigurationStates.DeviceStates)
					{
						if (chilDevice.PlaceInTree.StartsWith(deviceState.PlaceInTree) && chilDevice.PlaceInTree != deviceState.PlaceInTree)
						{
							var parentDeviceState = new ParentDeviceState()
							{
								ParentDeviceId = deviceState.UID,
								Code = state.Code,
								DriverState = state.DriverState,
								IsDeleting = false
							};

							var existingParentDeviceState = chilDevice.ParentStates.FirstOrDefault(x => x.ParentDeviceId == parentDeviceState.ParentDeviceId && x.Code == parentDeviceState.Code && x.DriverState == parentDeviceState.DriverState);
							if (existingParentDeviceState == null)
							{
								chilDevice.ParentStates.Add(parentDeviceState);
								ChangedDevices.Add(chilDevice);
							}
							else
							{
								existingParentDeviceState.IsDeleting = false;
							}
						}
					}
				}
			}

			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				for (int i = deviceState.ParentStates.Count(); i > 0; i--)
				{
					var parentState = deviceState.ParentStates[i - 1];
					if (parentState.IsDeleting)
					{
						deviceState.ParentStates.RemoveAt(i - 1);
						ChangedDevices.Add(deviceState);
					}
				}
			}
		}

		void PropogateStatesUp()
		{
			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				deviceState.ChildStates.ForEach(x => x.IsDeleting = true);
			}

			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				foreach (var state in deviceState.States.Where(x => x.DriverState.AffectedParent))
				{
					var parentDevice = deviceState.Device.Parent;
					var parentDeviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == parentDevice.UID);

					var childDeviceState = new ChildDeviceState()
					{
						ChildDeviceId = deviceState.UID,
						Code = state.Code,
						DriverState = state.DriverState,
						IsDeleting = false
					};

					var existingParentDeviceState = parentDeviceState.ChildStates.FirstOrDefault(x => x.ChildDeviceId == childDeviceState.ChildDeviceId && x.Code == childDeviceState.Code && x.DriverState == childDeviceState.DriverState);
					if (existingParentDeviceState == null)
					{
						parentDeviceState.ChildStates.Add(childDeviceState);
						ChangedDevices.Add(parentDeviceState);
					}
					else
					{
						existingParentDeviceState.IsDeleting = false;
					}
				}
			}

			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				for (int i = deviceState.ChildStates.Count(); i > 0; i--)
				{
					var childState = deviceState.ChildStates[i - 1];
					if (childState.IsDeleting)
					{
						deviceState.ChildStates.RemoveAt(i - 1);
						ChangedDevices.Add(deviceState);
					}
				}
			}
		}

		void CalculateZones()
		{
			try
			{
				if (FiresecManager.DeviceConfigurationStates.ZoneStates == null)
					return;

				foreach (var zoneState in FiresecManager.DeviceConfigurationStates.ZoneStates)
				{
					StateType minZoneStateType = StateType.Norm;
					var deviceStates = FiresecManager.DeviceConfigurationStates.DeviceStates.
						Where(x => x.Device.ZoneNo == zoneState.No && !x.Device.Driver.IgnoreInZoneState);

					foreach (var deviceState in deviceStates)
					{
						if (deviceState.StateType < minZoneStateType)
							minZoneStateType = deviceState.StateType;
					}

					if (FiresecManager.DeviceConfigurationStates.DeviceStates.Any(x => x.Device.ZoneNo == zoneState.No) == false)
						minZoneStateType = StateType.Unknown;

					if (zoneState.StateType != minZoneStateType)
					{
						zoneState.StateType = minZoneStateType;
						OnZoneStateChanged(zoneState);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Watcher.CalculateZones");
			}
		}

		Firesec.CoreState.devType FindDevice(Firesec.CoreState.devType[] innerDevices, string PlaceInTree)
		{
			return innerDevices != null ? innerDevices.FirstOrDefault(a => a.name == PlaceInTree) : null;
		}
	}
}