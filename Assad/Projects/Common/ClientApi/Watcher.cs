using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ServiceApi;
using Firesec;
using FiresecMetadata;

namespace ClientApi
{
    public class Watcher
    {
        internal void Start()
        {
            FiresecClient.NewEvent += new Action<int>(FiresecClient_NewEvent);

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

        void SetLastEvent()
        {
            Firesec.ReadEvents.document journal = FiresecClient.ReadEvents(0, 1);
            if ((journal.Journal != null) && (journal.Journal.Count() > 0))
            {
                LastEventId = Convert.ToInt32(journal.Journal[0].IDEvents);
            }
        }

        // ДОБАВИТЬ ПРОВЕРКУ - ЕСЛИ В ВЫЧИТАННЫХ 100 СОБЫТИЯХ ВСЕ СОБЫТИЯ НОВЫЕ, ТО ВЫЧИТАТЬ И ВТОРУЮ СОТНЮ

        void OnNewEvent()
        {
            Firesec.ReadEvents.document journal = FiresecClient.ReadEvents(0, 100);
            string journalString = FiresecClient.JournalString;

            if ((journal != null) && (journal.Journal.Count() > 0))
            {
                foreach (Firesec.ReadEvents.journalType journalItem in journal.Journal)
                {
                    if (Convert.ToInt32(journalItem.IDEvents) > LastEventId)
                    {
                        ServiceClient.NewJournalEvent(journalItem);
                    }
                }
                LastEventId = Convert.ToInt32(journal.Journal[0].IDEvents);
            }
        }

        void OnParametersChanged()
        {
            Firesec.DeviceParams.config coreParameters = FiresecClient.GetDeviceParams();
            try
            {
                Trace.WriteLine("OnParametersChanged");

                foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
                {
                    deviceState.ChangeEntities.Reset();

                    if (coreParameters.dev.Any(x => x.name == deviceState.PlaceInTree))
                    {
                        Firesec.DeviceParams.devType innerDevice = coreParameters.dev.FirstOrDefault(x => x.name == deviceState.PlaceInTree);

                        foreach (Parameter parameter in deviceState.Parameters)
                        {
                            if ((innerDevice.dev_param != null) && (innerDevice.dev_param.Any(x => x.name == parameter.Name)))
                            {
                                Firesec.DeviceParams.dev_paramType innerParameter = innerDevice.dev_param.FirstOrDefault(x => x.name == parameter.Name);
                                if (parameter.Value != innerParameter.value)
                                {
                                    deviceState.ChangeEntities.ParameterChanged = true;
                                    if (parameter.Visible)
                                        deviceState.ChangeEntities.VisibleParameterChanged = true;
                                }
                                parameter.Value = innerParameter.value;
                            }
                        }
                    }
                }

                CurrentStates currentStates = new CurrentStates();
                currentStates.DeviceStates = new List<DeviceState>();
                currentStates.ZoneStates = new List<ZoneState>();

                foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
                {
                    if (deviceState.ChangeEntities.ParameterChanged)
                    {
                        currentStates.DeviceStates.Add(deviceState);
                    }
                }

                ServiceClient.StateChanged(currentStates);
            }
            catch (Exception e)
            {
                Trace.WriteLine("OnParametersChanged Error: " + e.ToString());
            }
        }

        public void OnStateChanged()
        {
            Firesec.CoreState.config coreState = FiresecClient.GetCoreState();
            try
            {
                Trace.WriteLine("OnStateChanged");
                SetStates(coreState);
                PropogateStates();
                CalculateStates();
                CalculateZones();

                foreach (DeviceState device in ServiceClient.CurrentStates.DeviceStates)
                {
                    device.States = new List<string>();
                    foreach (string parentState in device.ParentStringStates)
                        device.States.Add(parentState);

                    foreach (string selfState in device.SelfStates)
                        device.States.Add(selfState);
                }

                CurrentStates currentStates = new CurrentStates();
                currentStates.DeviceStates = new List<DeviceState>();
                currentStates.ZoneStates = new List<ZoneState>();

                foreach (DeviceState device in ServiceClient.CurrentStates.DeviceStates)
                {
                    if ((device.ChangeEntities.StatesChanged) || (device.ChangeEntities.StateChanged))
                    {
                        currentStates.DeviceStates.Add(device);
                    }
                }

                foreach (ZoneState zone in ServiceClient.CurrentStates.ZoneStates)
                {
                    if (zone.ZoneChanged)
                    {
                        currentStates.ZoneStates.Add(zone);
                    }
                }

                ServiceClient.StateChanged(currentStates);
                Trace.WriteLine("OnStateChanged End");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Watcher Error: " + e.ToString());
            }
        }

        void SetStates(Firesec.CoreState.config coreState)
        {
            foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
            {
                Firesec.CoreState.devType innerDevice = FindDevice(coreState.dev, deviceState.PlaceInTree);

                bool hasOneActiveState = false;
                deviceState.SelfStates = new List<string>();

                if (innerDevice != null)
                {
                    foreach (InnerState state in deviceState.InnerStates)
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
                    foreach (InnerState state in deviceState.InnerStates)
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
            foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
            {
                deviceState.ParentInnerStates = new List<InnerState>();
                deviceState.ParentStringStates = new List<string>();
            }

            foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
            {
                foreach (InnerState state in deviceState.InnerStates)
                {
                    if ((state.IsActive) && (state.AffectChildren))
                    {
                        foreach (DeviceState chilDevice in ServiceClient.CurrentStates.DeviceStates)
                        {
                            if ((chilDevice.PlaceInTree.StartsWith(deviceState.PlaceInTree)) && (chilDevice.PlaceInTree != deviceState.PlaceInTree))
                            {
                                chilDevice.ParentInnerStates.Add(state);
                                string driverId = ServiceClient.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == deviceState.Path).DriverId;
                                string driverName = ServiceClient.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == driverId).shortName;
                                chilDevice.ParentStringStates.Add(driverName + " - " + state.Name);
                                chilDevice.ChangeEntities.StatesChanged = true;
                            }
                        }
                    }
                }
            }
        }

        void CalculateStates()
        {
            foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
            {
                int minPriority = 7;
                InnerState sourceState = null;

                foreach (InnerState state in deviceState.InnerStates)
                {
                    if (state.IsActive)
                    {
                        if (minPriority > state.Priority)
                            minPriority = state.Priority;
                    }
                }
                foreach (InnerState state in deviceState.ParentInnerStates)
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
                deviceState.State = StateHelper.GetState(minPriority);
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
            if (ServiceClient.CurrentStates.ZoneStates != null)
            {
                foreach (ZoneState zoneState in ServiceClient.CurrentStates.ZoneStates)
                {
                    int minZonePriority = 7;
                    foreach (Device device in ServiceClient.CurrentConfiguration.AllDevices)
                    {
                        if (device.ZoneNo == zoneState.No)
                        {
                            DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == device.Path);
                            // добавить проверку - нужно ли включать устройство при формировании состояния зоны
                            if (deviceState.MinPriority < minZonePriority)
                                minZonePriority = deviceState.MinPriority;
                        }
                    }

                    string newZoneState = StateHelper.GetState(minZonePriority);

                    if ((zoneState.State == null) || (zoneState.State != newZoneState))
                    {
                        zoneState.ZoneChanged = true;
                    }
                    else
                    {
                        zoneState.ZoneChanged = false;
                    }
                    zoneState.State = newZoneState;
                }
            }
        }

        Firesec.CoreState.devType FindDevice(Firesec.CoreState.devType[] innerDevices, string PlaceInTree)
        {
            Firesec.CoreState.devType innerDevice;
            if (innerDevices == null)
            {
                innerDevice = null;
            }
            else
            {
                try
                {
                    if (innerDevices.Any(a => a.name == PlaceInTree))
                    {
                        innerDevice = innerDevices.FirstOrDefault(a => a.name == PlaceInTree);
                    }
                    else
                    {
                        innerDevice = null;
                    }
                }
                catch
                {
                    innerDevice = null;
                }
            }
            return innerDevice;
        }
    }
}


//evmNewEvents           = $0001;
//evmStateChanged        = $0002;
//evmConfigChanged       = $0004;
//evmDeviceParamsUpdated = $0008;
//evmPong                = $0010;
//evmDatabaseChanged     = $0020;
//evmReportsChanged      = $0040;
//evmSoundsChanged       = $0080;
//evmLibraryChanged      = $0100;
//evmPing                = $0200;
//evmIgnoreListChanged   = $0400;
//evmEventViewChanged    = $0800;