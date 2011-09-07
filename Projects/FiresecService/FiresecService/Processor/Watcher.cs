using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    public class Watcher
    {
        internal void Start()
        {
            FiresecInternalClient.NewEvent += new Action<int>(FiresecClient_NewEvent);
            OnStateChanged();
            OnParametersChanged();
            SetLastEvent();
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
            {
                OnStateChanged();
            }
            if (evmDeviceParamsUpdated)
            {
                OnParametersChanged();
            }
            if (evmNewEvents)
            {
                OnNewEvent();
            }
        }

        int LastEventId = 0;
        List<DeviceState> ChangedDevices;

        void SetLastEvent()
        {
            Firesec.Journals.document journal = FiresecInternalClient.ReadEvents(0, 1);
            if ((journal.Journal != null) && (journal.Journal.Count() > 0))
            {
                LastEventId = int.Parse(journal.Journal[0].IDEvents);
            }
        }

        void OnNewEvent()
        {
            var document = FiresecInternalClient.ReadEvents(0, 100);

            if ((document != null) && (document.Journal.Count() > 0))
            {
                foreach (var innerJournalItem in document.Journal)
                {
                    if (int.Parse(innerJournalItem.IDEvents) > LastEventId)
                    {
                        var journalItem = JournalConverter.Convert(innerJournalItem);
                        CallbackManager.OnNewJournalRecord(journalItem);
                    }
                }
                LastEventId = int.Parse(document.Journal[0].IDEvents);
            }
        }

        void OnParametersChanged()
        {
            ChangedDevices = new List<DeviceState>();

            var coreParameters = FiresecInternalClient.GetDeviceParams();
            try
            {
                foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
                {
                    var innerDevice = coreParameters.dev.FirstOrDefault(x => x.name == deviceState.PlaceInTree);
                    if (innerDevice != null)
                    {
                        foreach (var parameter in deviceState.Parameters)
                        {
                            if ((innerDevice.dev_param != null) && (innerDevice.dev_param.Any(x => x.name == parameter.Name)))
                            {
                                var innerParameter = innerDevice.dev_param.FirstOrDefault(x => x.name == parameter.Name);
                                if (parameter.Value != innerParameter.value)
                                {
                                    if (parameter.Visible)
                                        ChangedDevices.Add(deviceState);
                                }
                                parameter.Value = innerParameter.value;
                            }
                        }
                    }
                }

                foreach (var deviceState in ChangedDevices)
                {
                    CallbackManager.OnDeviceParametersChanged(deviceState);
                }
            }
            catch (Exception) { }
        }

        public void OnStateChanged()
        {
            ChangedDevices = new List<DeviceState>();

            var coreState = FiresecInternalClient.GetCoreState();
            try
            {
                SetStates(coreState);
                PropogateStates();
                CalculateStates();
                CalculateZones();

                foreach (var deviceState in ChangedDevices)
                {
                    CallbackManager.OnDeviceStateChanged(deviceState);
                }
            }
            catch (Exception) { }
        }

        void SetStates(Firesec.CoreState.config coreState)
        {
            foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
            {
                Firesec.CoreState.devType innerDevice = FindDevice(coreState.dev, deviceState.PlaceInTree);

                bool hasOneChangedState = false;

                if (innerDevice != null)
                {
                    foreach (var state in deviceState.States)
                    {
                        bool IsActive = innerDevice.state.Any(a => a.id == state.DriverState.Id);
                        if (state.IsActive != IsActive)
                        {
                            hasOneChangedState = true;
                        }
                        state.IsActive = IsActive;
                    }
                }
                else
                {
                    foreach (var state in deviceState.States)
                    {
                        if (state.IsActive)
                        {
                            hasOneChangedState = true;
                        }
                        state.IsActive = false;
                    }
                }

                if (hasOneChangedState)
                {
                    ChangedDevices.Add(deviceState);
                }
            }
        }

        void PropogateStates()
        {
            foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
            {
                deviceState.ParentStates = new List<ParentDeviceState>();
            }

            foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
            {
                foreach (var state in deviceState.States)
                {
                    if ((state.IsActive) && (state.DriverState.AffectChildren))
                    {
                        foreach (var chilDevice in FiresecManager.DeviceConfigurationStates.DeviceStates)
                        {
                            if ((chilDevice.PlaceInTree.StartsWith(deviceState.PlaceInTree)) && (chilDevice.PlaceInTree != deviceState.PlaceInTree))
                            {
                                var parentDeviceState = new ParentDeviceState();
                                parentDeviceState.ParentDeviceId = deviceState.UID;
                                parentDeviceState.Code = state.Code;
                                parentDeviceState.DriverState = state.DriverState;
                                chilDevice.ParentStates.Add(parentDeviceState);
                                ChangedDevices.Add(chilDevice);
                            }
                        }
                    }
                }
            }
        }

        void CalculateStates()
        {
            foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
            {
                StateType minStateClassId = StateType.Norm;

                foreach (var state in deviceState.States)
                {
                    if (state.IsActive)
                    {
                        if (minStateClassId > state.DriverState.StateType)
                            minStateClassId = state.DriverState.StateType;
                    }
                }
                foreach (var state in deviceState.ParentStates)
                {
                    if (minStateClassId > state.DriverState.StateType)
                    {
                        minStateClassId = state.DriverState.StateType;
                    }
                }

                if (deviceState.StateType != minStateClassId)
                {
                    ChangedDevices.Add(deviceState);
                }
            }
        }

        void CalculateZones()
        {
            if (FiresecManager.DeviceConfigurationStates.ZoneStates != null)
            {
                foreach (var zoneState in FiresecManager.DeviceConfigurationStates.ZoneStates)
                {
                    var zoneHasDevices = false;
                    StateType minZoneStateType = StateType.Norm;
                    foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                    {
                        if (device.ZoneNo == zoneState.No)
                        {
                            zoneHasDevices = true;
                            var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == device.Id);
                            // добавить проверку - нужно ли включать устройство при формировании состояния зоны
                            if (deviceState.StateType < minZoneStateType)
                                minZoneStateType = deviceState.StateType;
                        }
                    }

                    if (zoneHasDevices == false)
                    {
                        minZoneStateType = StateType.Unknown;
                    }

                    bool zoneChanged = (zoneState.StateType != minZoneStateType);
                    zoneState.StateType = minZoneStateType;

                    if (zoneChanged)
                    {
                        CallbackManager.OnZoneStateChanged(zoneState);
                    }
                }
            }
        }

        Firesec.CoreState.devType FindDevice(Firesec.CoreState.devType[] innerDevices, string PlaceInTree)
        {
            if (innerDevices != null)
            {
                return innerDevices.FirstOrDefault(a => a.name == PlaceInTree);
            }
            return null;
        }
    }
}