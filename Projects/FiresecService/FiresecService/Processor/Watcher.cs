using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec;
using FiresecAPI.Models;
using FiresecService.Converters;
using System.Diagnostics;

namespace FiresecService
{
    public class Watcher
    {
        public void Start()
        {
            FiresecInternalClient.NewEvent += new Action<int>(FiresecClient_NewEvent);
            FiresecInternalClient.Progress += new FiresecEventAggregator.ProgressDelegate(FiresecInternalClient_Progress);
            OnStateChanged();
            OnParametersChanged();
            SetLastEvent();
        }

        bool FiresecInternalClient_Progress(int stage, string comment, int percentComplete, int bytesRW)
        {
            return CallbackManager.OnProgress(stage, comment, percentComplete, bytesRW);
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
            Firesec.Journals.document journal = FiresecInternalClient.ReadEvents(0, 1);
            if (journal != null && journal.Journal.IsNotNullOrEmpty())
            {
                LastEventId = int.Parse(journal.Journal[0].IDEvents);
            }
        }

        void OnNewEvent()
        {
            var document = FiresecInternalClient.ReadEvents(0, 100);
            if (document != null && document.Journal.IsNotNullOrEmpty())
            {
                foreach (var innerJournalItem in document.Journal)
                {
                    if (int.Parse(innerJournalItem.IDEvents) > LastEventId)
                    {
                        var journalRecord = JournalConverter.Convert(innerJournalItem);
                        CallbackManager.OnNewJournalRecord(journalRecord);
                        DatabaseHelper.AddJournalRecord(journalRecord);
                    }
                }
                LastEventId = int.Parse(document.Journal[0].IDEvents);
            }
        }

        void OnParametersChanged()
        {
            ChangedDevices = new HashSet<DeviceState>();
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
                            if (innerDevice.dev_param != null &&
                                innerDevice.dev_param.Any(x => x.name == parameter.Name))
                            {
                                var innerParameter = innerDevice.dev_param.FirstOrDefault(x => x.name == parameter.Name);
                                if (parameter.Value != innerParameter.value && parameter.Visible)
                                {
                                    ChangedDevices.Add(deviceState);
                                }
                                parameter.Value = innerParameter.value;
                            }
                        }
                    }
                }

                if (ChangedDevices.Count > 0)
                    CallbackManager.OnDeviceParametersChanged(ChangedDevices.ToList());
            }
            catch (Exception) { }
        }

        public void OnStateChanged()
        {
            ChangedDevices = new HashSet<DeviceState>();
            var coreState = FiresecInternalClient.GetCoreState();
            try
            {
                SetStates(coreState);
                PropogateStates();
                CalculateZones();

                if (ChangedDevices.Count > 0)
                    CallbackManager.OnDeviceStatesChanged(ChangedDevices.ToList());
            }
            catch (Exception) { }
        }

        void SetStates(Firesec.CoreState.config coreState)
        {
            foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
            {
                bool hasOneChangedState = false;

                Firesec.CoreState.devType innerDevice = FindDevice(coreState.dev, deviceState.PlaceInTree);
                if (innerDevice != null)
                {
                    foreach (var driverState in deviceState.Device.Driver.States)
                    {
                        var innerState = innerDevice.state.FirstOrDefault(a => a.id == driverState.Id);
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

        void PropogateStates()
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

        void CalculateZones()
        {
            if (FiresecManager.DeviceConfigurationStates.ZoneStates == null)
                return;

            foreach (var zoneState in FiresecManager.DeviceConfigurationStates.ZoneStates)
            {
                StateType minZoneStateType = StateType.Norm;
                foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates.
                    Where(x => x.Device.ZoneNo == zoneState.No && !x.Device.Driver.IgnoreInZoneState))
                {
                    if (deviceState.StateType < minZoneStateType)
                        minZoneStateType = deviceState.StateType;
                }

                if (FiresecManager.DeviceConfigurationStates.DeviceStates.
                    Any(x => x.Device.ZoneNo == zoneState.No) == false)
                    minZoneStateType = StateType.Unknown;

                if (zoneState.StateType != minZoneStateType)
                {
                    zoneState.StateType = minZoneStateType;
                    CallbackManager.OnZoneStateChanged(zoneState);
                }
            }
        }

        Firesec.CoreState.devType FindDevice(Firesec.CoreState.devType[] innerDevices, string PlaceInTree)
        {
            return innerDevices != null ? innerDevices.FirstOrDefault(a => a.name == PlaceInTree) : null;
        }
    }
}