using System;
using System.Collections.Generic;
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
				FiresecSerializedClient.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
				FiresecSerializedClient.StateChanged += new Action<Models.CoreState.config>(OnStateChanged);
				FiresecSerializedClient.ParametersChanged += new Action<Models.DeviceParameters.config>(OnParametersChanged);
			}
			FiresecSerializedClient.ProgressEvent += new Action<int, string, int, int>(OnProgress);
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
			for (int i = 0; i < 100; i++ )
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
				if (!hasNewRecords)
					break;
			}

			return result;
		}

		public void ForceParametersChanged()
		{
			var coreParameters = FiresecSerializedClient.GetDeviceParams();
			if (coreParameters != null && coreParameters.Result != null)
			{
				OnParametersChanged(coreParameters.Result);
			}
            else
            {
                throw new FiresecException("Список параметров драйвера пуст");
            }
		}

		public void OnParametersChanged(Firesec.Models.DeviceParameters.config coreParameters)
		{
			ChangedDevices = new HashSet<DeviceState>();
			ChangedZones = new HashSet<ZoneState>();
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
							if (innerDevice.dev_param != null && innerDevice.dev_param.Any(x => x.name == parameter.Name))
							{
								var innerParameter = innerDevice.dev_param.FirstOrDefault(x => x.name == parameter.Name);
								if (innerParameter != null)
								{
									if (parameter.Value != innerParameter.value)
									{
										ChangedDevices.Add(device.DeviceState);
									}
									parameter.Value = innerParameter.value;
								}
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
			Current.OnStateChanged(coreState);
		}

		public void ForceStateChanged()
		{
			var coreState = FiresecSerializedClient.GetCoreState();
            if (coreState != null && coreState.Result != null)
            {
                OnStateChanged(coreState.Result);
            }
            else
            {
                throw new FiresecException("Список состояний драйвера пуст");
            }
		}

		void OnStateChanged(Firesec.Models.CoreState.config coreState)
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
            if (coreState == null || coreState.dev == null)
            {
                return;
            }

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				if (device.PlaceInTree == null)
				{
					Logger.Error("Watcher.SetStates deviceState.PlaceInTree = null");
					continue;
				}

				bool hasOneChangedState = false;

				Firesec.Models.CoreState.devType innerDevice = FindDevice(coreState.dev, device.PlaceInTree);
				if (innerDevice != null)
				{
					if (device.Driver == null)
					{
						Logger.Error("Watcher.SetStates deviceState.Device.Driver = null");
                        continue;
					}
					if (device.Driver.States == null)
					{
						Logger.Error("Watcher.SetStates deviceState.Device.Driver.States = null");
                        continue;
					}
                    if (innerDevice.state == null)
                    {
						innerDevice.state = (new List<Firesec.Models.CoreState.stateType>()).ToArray();
                    }

					foreach (var driverState in device.Driver.States)
					{
						var innerState = innerDevice.state.FirstOrDefault(a => a.id == driverState.Id);
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
						if ((childDevice.PlaceInTree + "/").StartsWith(device.PlaceInTree) && childDevice.PlaceInTree != device.PlaceInTree)
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
				if (device.Driver.ChildAddressReserveRangeCount == 0)
					continue;

				ChildDeviceState childDeviceState = null;
				var minChildStateType = StateType.Norm;
				foreach (var child in device.Children)
				{
					if (child.DeviceState.StateType < minChildStateType)
					{
						minChildStateType = child.DeviceState.StateType;
						childDeviceState = new ChildDeviceState()
						{
							ChildDevice = device,
							StateType = minChildStateType,
							IsDeleting = false
						};
					}
				}

				if (childDeviceState == null)
					continue;

				var existingDeviceState = device.DeviceState.ChildStates.FirstOrDefault(x => x.ChildDevice.UID == childDeviceState.ChildDevice.UID && x.StateType == childDeviceState.StateType);
				if (existingDeviceState == null)
				{
					device.DeviceState.ChildStates.Add(childDeviceState);
					ChangedDevices.Add(device.DeviceState);
				}
				else
				{
					existingDeviceState.IsDeleting = false;
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

        public event Action<int, string, int, int> Progress;
		void OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (Progress != null)
				Progress(stage, comment, percentComplete, bytesRW);
		}
	}
}