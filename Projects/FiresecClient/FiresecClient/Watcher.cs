using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Firesec;
using System.IO;
using System.Xml.Serialization;
using FiresecClient.Models;

namespace FiresecClient
{
    public class Watcher
    {
        internal void Start()
        {
            FiresecInternalClient.NewEvent += new Action<int>(FiresecClient_NewEvent);
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
                var state = FiresecInternalClient.GetCoreState();
                OnStateChanged(state);
            }
            if (evmDeviceParamsUpdated)
            {
                var parameters = FiresecInternalClient.GetDeviceParams();
                OnParametersChanged(parameters);
            }
            if (evmNewEvents)
            {
                var document = FiresecInternalClient.ReadEvents(0, 100);
                OnNewEvent(document);
            }
        }

        int LastEventId = 0;

        void SetLastEvent()
        {
            Firesec.ReadEvents.document journal = FiresecInternalClient.ReadEvents(0, 1);
            if ((journal.Journal != null) && (journal.Journal.Count() > 0))
            {
                LastEventId = Convert.ToInt32(journal.Journal[0].IDEvents);
            }
        }

        // ДОБАВИТЬ ПРОВЕРКУ - ЕСЛИ В ВЫЧИТАННЫХ 100 СОБЫТИЯХ ВСЕ СОБЫТИЯ НОВЫЕ, ТО ВЫЧИТАТЬ И ВТОРУЮ СОТНЮ

        void OnNewEvent(Firesec.ReadEvents.document journal)
        {
            if ((journal != null) && (journal.Journal.Count() > 0))
            {
                foreach (var journalItem in journal.Journal)
                {
                    if (Convert.ToInt32(journalItem.IDEvents) > LastEventId)
                    {
                        CurrentStates.OnNewJournalEvent(journalItem);
                    }
                }
                LastEventId = Convert.ToInt32(journal.Journal[0].IDEvents);
            }
        }

        void OnParametersChanged(Firesec.DeviceParams.config coreParameters)
        {
            try
            {
                Trace.WriteLine("OnParametersChanged");

                foreach (var deviceState in FiresecManager.States.DeviceStates)
                {
                    deviceState.ChangeEntities.Reset();

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
                                    deviceState.ChangeEntities.ParameterChanged = true;
                                    if (parameter.Visible)
                                        deviceState.ChangeEntities.VisibleParameterChanged = true;
                                }
                                parameter.Value = innerParameter.value;
                            }
                        }

                        if (deviceState.ChangeEntities.ParameterChanged)
                        {
                            FiresecManager.States.OnDeviceParametersChanged(deviceState.Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("OnParametersChanged Error: " + e.ToString());
            }
        }

        public void OnStateChanged(Firesec.CoreState.config coreState)
        {
            try
            {
                SetStates(coreState);
                PropogateStates();
                CalculateStates();
                CalculateZones();

                foreach (var device in FiresecManager.States.DeviceStates)
                {
                    device.States = new List<string>();
                    foreach (var parentState in device.ParentStringStates)
                        device.States.Add(parentState);

                    foreach (var selfState in device.SelfStates)
                        device.States.Add(selfState);
                }

                foreach (var device in FiresecManager.States.DeviceStates)
                {
                    if ((device.ChangeEntities.StatesChanged) || (device.ChangeEntities.StateChanged))
                    {
                        device.OnStateChanged();
                        FiresecManager.States.OnDeviceStateChanged(device.Id);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Watcher Error: " + e.ToString());
            }
        }

        void SetStates(Firesec.CoreState.config coreState)
        {
            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                Firesec.CoreState.devType innerDevice = FindDevice(coreState.dev, deviceState.PlaceInTree);

                bool hasOneActiveState = false;
                deviceState.SelfStates = new List<string>();

                if (innerDevice != null)
                {
                    foreach (var state in deviceState.InnerStates)
                    {
                        bool IsActive = innerDevice.state.Any(a => a.id == state.Id);
                        if (state.IsActive != IsActive)
                        {
                            hasOneActiveState = true;
                            deviceState.SelfStates.Add(state.Name);
                        }
                        state.IsActive = IsActive;
                    }
                }
                else
                {
                    foreach (var state in deviceState.InnerStates)
                    {
                        if (state.IsActive)
                        {
                            hasOneActiveState = true;
                        }
                        state.IsActive = false;
                    }
                }

                if (hasOneActiveState)
                {
                    deviceState.ChangeEntities.StatesChanged = true;
                }
                else
                {
                    deviceState.ChangeEntities.StatesChanged = false;
                }
            }
        }

        void PropogateStates()
        {
            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                deviceState.ParentInnerStates = new List<InnerState>();
                deviceState.ParentStringStates = new List<string>();
            }

            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                foreach (var state in deviceState.InnerStates)
                {
                    if ((state.IsActive) && (state.AffectChildren))
                    {
                        foreach (var chilDevice in FiresecManager.States.DeviceStates)
                        {
                            if ((chilDevice.PlaceInTree.StartsWith(deviceState.PlaceInTree)) && (chilDevice.PlaceInTree != deviceState.PlaceInTree))
                            {
                                chilDevice.ParentInnerStates.Add(state);
                                var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == deviceState.Id);
                                chilDevice.ParentStringStates.Add(device.Driver.ShortName + " - " + state.Name);
                                chilDevice.ChangeEntities.StatesChanged = true;
                            }
                        }
                    }
                }
            }
        }

        void CalculateStates()
        {
            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                int minPriority = 7;
                InnerState sourceState = null;

                foreach (var state in deviceState.InnerStates)
                {
                    if (state.IsActive)
                    {
                        if (minPriority > state.Priority)
                            minPriority = state.Priority;
                    }
                }
                foreach (var state in deviceState.ParentInnerStates)
                {
                    if (state.IsActive)
                    {
                        if (minPriority > state.Priority)
                        {
                            minPriority = state.Priority;
                            sourceState = state;
                        }
                    }
                }
                if (deviceState.MinPriority != minPriority)
                {
                    deviceState.ChangeEntities.StateChanged = true;
                }
                else
                {
                    deviceState.ChangeEntities.StateChanged = false;
                }
                deviceState.State = new State(minPriority);
                deviceState.MinPriority = minPriority;

                if (sourceState != null)
                {
                    deviceState.SourceState = sourceState.Name;
                }
                else
                {
                    deviceState.SourceState = "";
                }
            }
        }

        void CalculateZones()
        {
            if (FiresecManager.States.ZoneStates != null)
            {
                foreach (var zoneState in FiresecManager.States.ZoneStates)
                {
                    int minZonePriority = 7;
                    foreach (var device in FiresecManager.Configuration.Devices)
                    {
                        if (device.ZoneNo == zoneState.No)
                        {
                            DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == device.Id);
                            // добавить проверку - нужно ли включать устройство при формировании состояния зоны
                            if (deviceState.MinPriority < minZonePriority)
                                minZonePriority = deviceState.MinPriority;
                        }
                    }

                    State newZoneState = new State(minZonePriority);

                    bool ZoneChanged = false;

                    if ((zoneState.State == null) || (zoneState.State != newZoneState))
                    {
                        ZoneChanged = true;
                    }
                    zoneState.State = newZoneState;

                    if (ZoneChanged)
                    {
                        FiresecManager.States.OnZoneStateChanged(zoneState.No);
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
