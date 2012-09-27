using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace Firesec
{
	public class Watcher
	{
        internal static Watcher Current;
		FiresecSerializedClient FiresecSerializedClient;
		int LastJournalNo = 0;
		HashSet<DeviceState> ChangedDevices;
		HashSet<ZoneState> ChangedZones;
        bool MustMonitorJournal;

        public Watcher(FiresecSerializedClient firesecSerializedClient, bool mustMonitorStates, bool mustMonitorJournal)
		{
            Current = this;
			FiresecSerializedClient = firesecSerializedClient;
            MustMonitorJournal = mustMonitorJournal;
            if (mustMonitorJournal)
			{
				SetLastEvent();
            }
            if (mustMonitorStates)
            {
				OnStateChanged();
				OnParametersChanged();
				FiresecSerializedClient.NativeFiresecClient.NewEventAvaliable += new Action<int>(FiresecClient_NewEvent);
			}
            FiresecSerializedClient.NativeFiresecClient.ProgressEvent += new Func<int, string, int, int, bool>(OnProgress);
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
                if (MustMonitorJournal)
                {
                    OnNewEvent();
                }
		}

		void SetLastEvent()
		{
			Firesec.Models.Journals.document journal = FiresecSerializedClient.ReadEvents(0, 100).Result;
			if (journal != null && journal.Journal.IsNotNullOrEmpty())
			{
				foreach (var journalItem in journal.Journal)
				{
					var intValue = int.Parse(journalItem.IDEvents);
					if (intValue > LastJournalNo)
						LastJournalNo = intValue;
				}
			}
		}

		public List<JournalRecord> SynchrinizeJournal(int oldJournalNo)
		{
			if (oldJournalNo >= 0)
			{
				var journalRecords = GetEventsFromLastId(oldJournalNo);
				return journalRecords;
			}
			return new List<JournalRecord>();
		}

		List<JournalRecord> GetEventsFromLastId(int oldJournalNo)
		{
			var result = new List<JournalRecord>();

			var hasNewRecords = true;
			//while (hasNewRecords)
			{
				hasNewRecords = false;
				var document = FiresecSerializedClient.ReadEvents(oldJournalNo, 100).Result;
				if (document != null && document.Journal.IsNotNullOrEmpty())
				{
					foreach (var innerJournalItem in document.Journal)
					{
						var eventId = int.Parse(innerJournalItem.IDEvents);
						if (eventId > oldJournalNo)
						{
							LastJournalNo = eventId;
							oldJournalNo = eventId;
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
			var journalRecords = GetEventsFromLastId(LastJournalNo);
			if (journalRecords.Count > 0)
			{
				OnNewJournalRecords(journalRecords);
			}
		}

		public void OnParametersChanged()
		{
			ChangedDevices = new HashSet<DeviceState>();
			ChangedZones = new HashSet<ZoneState>();
			var coreParameters = FiresecSerializedClient.GetDeviceParams().Result;
			if (coreParameters == null)
				return;
			if (coreParameters.dev == null)
				return;

			try
			{
				foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
				{
					var innerDevice = coreParameters.dev.FirstOrDefault(x => x.name == device.PlaceInTree);
					if (innerDevice != null)
					{
						foreach (var parameter in device.DeviceState.Parameters)
						{
							if (innerDevice.dev_param != null &&
								innerDevice.dev_param.Any(x => x.name == parameter.Name))
							{
								var innerParameter = innerDevice.dev_param.FirstOrDefault(x => x.name == parameter.Name);
								if (parameter.Value != innerParameter.value)
								{
									ChangedDevices.Add(device.DeviceState);
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

		internal static void ImitatorStateChanged(Firesec.Models.CoreState.config coreState)
		{
			Current.StateChanged(coreState);
		}

		public void OnStateChanged()
		{
			var coreState = FiresecSerializedClient.GetCoreState();
			if (coreState != null && coreState.Result != null)
			{
				StateChanged(coreState.Result);
			}
		}

		void StateChanged(Firesec.Models.CoreState.config coreState)
		{
			try
			{
				ChangedDevices = new HashSet<DeviceState>();
				ChangedZones = new HashSet<ZoneState>();

				SetStates(coreState);
				PropogateStatesDown();
				PropogateStatesUp();
				CalculateZones();

				if (ChangedDevices.Count > 0)
				{
					OnDevicesStateChanged(ChangedDevices.ToList());
				}

				if (ChangedZones.Count > 0)
				{
					OnZonesStateChanged(ChangedZones.ToList());
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Watcher.OnStateChanged");
			}
		}

		void SetStates(Firesec.Models.CoreState.config coreState)
		{
			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				if (coreState == null)
				{
					Logger.Error("Watcher.SetStates coreState = null");
					return;
				}
				if (coreState.dev == null)
				{
					//Logger.Error("Watcher.SetStates coreState.dev = null");
					return;
				}
				if (device.PlaceInTree == null)
				{
					Logger.Error("Watcher.SetStates deviceState.PlaceInTree = null");
					return;
				}

				bool hasOneChangedState = false;

				Firesec.Models.CoreState.devType innerDevice = FindDevice(coreState.dev, device.PlaceInTree);
				if (innerDevice != null)
				{
					if (device.Driver == null)
					{
						Logger.Error("Watcher.SetStates deviceState.Device.Driver = null");
						return;
					}
					if (device.Driver.States == null)
					{
						Logger.Error("Watcher.SetStates deviceState.Device.Driver.States = null");
						return;
					}

					foreach (var driverState in device.Driver.States)
					{
						if (innerDevice.state == null)
						{
							Logger.Error("Watcher.SetStates innerDevice.state = null");
							return;
						}
						var innerState = innerDevice.state.FirstOrDefault(a => a.id == driverState.Id);
						if (device.DeviceState.States == null)
						{
							Logger.Error("Watcher.SetStates deviceState.States = null");
							return;
						}
						var state = device.DeviceState.States.FirstOrDefault(x => x.DriverState.Code == driverState.Code);
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
									DriverState = driverState
								};
								device.DeviceState.States.Add(state);
							}

							if (innerState.time != null)
								state.Time = JournalConverter.ConvertTime(innerState.time);
							else
								state.Time = null;
						}
						else
						{
							if (state != null)
								device.DeviceState.States.Remove(state);
						}
					}
				}
				else
				{
					hasOneChangedState = device.DeviceState.States.Count > 0;
					device.DeviceState.States.Clear();
				}

				if (hasOneChangedState)
				{
					ChangedDevices.Add(device.DeviceState);
				}
			}
		}

		void PropogateStatesDown()
		{
			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				device.DeviceState.ParentStates.ForEach(x => x.IsDeleting = true);
			}

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				foreach (var state in device.DeviceState.States.Where(x => x.DriverState.AffectChildren))
				{
					foreach (var childDevice in ConfigurationCash.DeviceConfiguration.Devices)
					{
						if (childDevice.PlaceInTree == null)
						{
							Logger.Error("Watcher.PropogateStatesDown chilDevice.PlaceInTree = null");
							continue;
						}
						if (childDevice.PlaceInTree.StartsWith(device.PlaceInTree) && childDevice.PlaceInTree != device.PlaceInTree)
						{
							var parentDeviceState = new ParentDeviceState()
							{
								ParentDevice = device,
								DriverState = state.DriverState,
								IsDeleting = false
							};

							var existingParentDeviceState = childDevice.DeviceState.ParentStates.FirstOrDefault(x => x.ParentDevice.UID == parentDeviceState.ParentDevice.UID && x.DriverState.Code == parentDeviceState.DriverState.Code && x.DriverState == parentDeviceState.DriverState);
							if (existingParentDeviceState == null)
							{
								childDevice.DeviceState.ParentStates.Add(parentDeviceState);
								ChangedDevices.Add(childDevice.DeviceState);
							}
							else
							{
								existingParentDeviceState.IsDeleting = false;
							}
						}
					}
				}
			}

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				for (int i = device.DeviceState.ParentStates.Count(); i > 0; i--)
				{
					var parentState = device.DeviceState.ParentStates[i - 1];
					if (parentState.IsDeleting)
					{
						device.DeviceState.ParentStates.RemoveAt(i - 1);
						ChangedDevices.Add(device.DeviceState);
					}
				}
			}
		}

		void PropogateStatesUp()
		{
			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				if (device.DeviceState.ChildStates == null)
				{
					Logger.Error("Watcher.PropogateStatesUp deviceState.ChildStates = null");
					return;
				}
				device.DeviceState.ChildStates.ForEach(x => x.IsDeleting = true);
			}

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				foreach (var state in device.DeviceState.States)
				{
					if (device.Parent == null)
						continue;
					if (device.Parent.Driver.ChildAddressReserveRangeCount == 0 || state.DriverState.AffectParent == false)
						continue;

					var parentDeviceState = device.Parent.DeviceState;

					var childDeviceState = new ChildDeviceState()
					{
						ChildDevice = device,
						DriverState = state.DriverState,
						IsDeleting = false
					};

					var existingParentDeviceState = parentDeviceState.ChildStates.FirstOrDefault(x => x.ChildDevice.UID == childDeviceState.ChildDevice.UID && x.DriverState.Code == childDeviceState.DriverState.Code && x.DriverState == childDeviceState.DriverState);
					if (existingParentDeviceState == null)
					{
						parentDeviceState.ChildStates.Add(childDeviceState);
						Trace.WriteLine(parentDeviceState.Device.PresentationAddressAndDriver + " " + childDeviceState.DriverState.Name);
						ChangedDevices.Add(parentDeviceState);
					}
					else
					{
						existingParentDeviceState.IsDeleting = false;
					}
				}
			}

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				for (int i = device.DeviceState.ChildStates.Count(); i > 0; i--)
				{
					var childState = device.DeviceState.ChildStates[i - 1];
					if (childState.IsDeleting)
					{
						device.DeviceState.ChildStates.RemoveAt(i - 1);
						ChangedDevices.Add(device.DeviceState);
					}
				}
			}
		}

		void CalculateZones()
		{
			try
			{
				foreach (var zone in ConfigurationCash.DeviceConfiguration.Zones)
				{
					StateType minZoneStateType = StateType.Norm;
					var devices = ConfigurationCash.DeviceConfiguration.Devices.
                        Where(x => x.ZoneUID == zone.UID && !x.Driver.IgnoreInZoneState);

					foreach (var device in devices)
					{
						if (device.DeviceState.StateType < minZoneStateType)
							minZoneStateType = device.DeviceState.StateType;
					}

                    if (ConfigurationCash.DeviceConfiguration.Devices.Any(x => x.ZoneUID == zone.UID) == false)
						minZoneStateType = StateType.Unknown;

					if (zone.ZoneState.StateType != minZoneStateType)
					{
						zone.ZoneState.StateType = minZoneStateType;
						ChangedZones.Add(zone.ZoneState);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Watcher.CalculateZones");
			}
		}

		Firesec.Models.CoreState.devType FindDevice(Firesec.Models.CoreState.devType[] innerDevices, string PlaceInTree)
		{
			return innerDevices != null ? innerDevices.FirstOrDefault(a => a.name == PlaceInTree) : null;
		}

		public event Action<List<DeviceState>> DevicesStateChanged;
		void OnDevicesStateChanged(List<DeviceState> deviceStates)
		{
			if (DevicesStateChanged != null)
				DevicesStateChanged(deviceStates);
		}

		public event Action<List<DeviceState>> DevicesParametersChanged;
		void OnDevicesParametersChanged(List<DeviceState> deviceStates)
		{
			if (DevicesParametersChanged != null)
				DevicesParametersChanged(deviceStates);
		}

		public event Action<List<ZoneState>> ZonesStateChanged;
		void OnZonesStateChanged(List<ZoneState> zoneStates)
		{
			if (ZonesStateChanged != null)
				ZonesStateChanged(zoneStates);
		}

		public event Action<List<JournalRecord>> NewJournalRecords;
		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			if (NewJournalRecords != null)
				NewJournalRecords(journalRecords);
		}

        public event Func<int, string, int, int, bool> Progress;
		bool OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (Progress != null)
				return Progress(stage, comment, percentComplete, bytesRW);
            return true;
		}
	}
}