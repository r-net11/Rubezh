using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiseProcessor;
using System.Diagnostics;
using ServiceApi;
using Firesec;
using FiresecMetadata;

namespace ServiseProcessor
{
    public class Watcher
    {
        internal void Start()
        {
            Firesec.FiresecEventAggregator.StateChanged += new Action<string, Firesec.CoreState.config>(OnStateChanged);
            Firesec.FiresecEventAggregator.ParametersChanged += new Action<string, Firesec.DeviceParams.config>(OnParametersChanged);
            Firesec.FiresecEventAggregator.NewEvent += new Action(FiresecEventAggregator_NewEvent);


            // первый опрос не должен выкидывать события сервера
            Firesec.CoreState.config coreState = Firesec.FiresecClient.GetCoreState();
            OnStateChanged(Firesec.FiresecClient.CoreStateString, coreState);

            Firesec.DeviceParams.config coreParameters = Firesec.FiresecClient.GetDeviceParams();
            OnParametersChanged(FiresecClient.DeviceParametersString, coreParameters);

            SetLastEvent();
        }

        int LastEventId = 0;

        void SetLastEvent()
        {
            Firesec.ReadEvents.document journal = FiresecClient.ReadEvents(0, 1);
            if (journal.Journal != null)
            {
                if (journal.Journal.Count() > 0)
                {
                    int lastEvent = Convert.ToInt32(journal.Journal[0].IDEvents);
                }
            }
        }

        void FiresecEventAggregator_NewEvent()
        {
            Firesec.ReadEvents.document journal = FiresecClient.ReadEvents(0, 100);
            string journalString = FiresecClient.JournalString;

            Firesec.ReadEvents.journalType journalItem = journal.Journal[0];

            string dataBaseId = null;

            if (journalItem.IDDevices != null)
            {
                dataBaseId = journalItem.IDDevices;
            }
            if (journalItem.IDDevicesSource != null)
            {
                dataBaseId = journalItem.IDDevices;
            }
            if (dataBaseId != null)
            {
                if (Services.AllDevices.Any(x => x.DatabaseId == dataBaseId))
                {
                    Device device = Services.AllDevices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
                    DeviceState deviceState = Services.CurrentStates.DeviceStates.FirstOrDefault(x=>x.Path == device.Path);
                    deviceState.ChangeEntities.IsNewEvent = true;
                    deviceState.ChangeEntities.StateChanged = false;
                    deviceState.ChangeEntities.StatesChanged = false;
                    deviceState.ChangeEntities.ParameterChanged = false;
                    deviceState.ChangeEntities.VisibleParameterChanged = false;

                    CurrentStates currentStates = new CurrentStates();
                    currentStates.DeviceStates = new List<DeviceState>();
                    currentStates.ZoneStates = new List<ZoneState>();
                    currentStates.DeviceStates.Add(deviceState);
                    StateService.StatesChanged(currentStates);
                }
            }

            ////int EventId = Convert.ToInt32(journal.Journal[0].IDEvents);
            ////Trace.WriteLine("Last Event Id: " + EventId.ToString());
            ////if (EventId > LastEventId)
            ////    Trace.WriteLine("NEW EVENT: " + EventId.ToString());
        }

        void OnParametersChanged(string coreParametersString, Firesec.DeviceParams.config coreParameters)
        {
            try
            {
                Trace.WriteLine("OnParametersChanged");

                foreach (DeviceState deviceState in Services.CurrentStates.DeviceStates)
                {
                    deviceState.ChangeEntities.StateChanged = false;
                    deviceState.ChangeEntities.StatesChanged = false;
                    deviceState.ChangeEntities.ParameterChanged = false;
                    deviceState.ChangeEntities.VisibleParameterChanged = false;
                    deviceState.ChangeEntities.IsNewEvent = false;

                    if (coreParameters.dev.Any(x => x.name == deviceState.PlaceInTree))
                    {
                        Firesec.DeviceParams.devType innerDevice = coreParameters.dev.FirstOrDefault(x => x.name == deviceState.PlaceInTree);
                        foreach (Parameter parameter in deviceState.Parameters)
                        {
                            if (innerDevice.dev_param != null)
                            {
                                if (innerDevice.dev_param.Any(x => x.name == parameter.Name))
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
                }

                CurrentStates currentStates = new CurrentStates();
                currentStates.DeviceStates = new List<DeviceState>();
                currentStates.ZoneStates = new List<ZoneState>();

                foreach (DeviceState deviceState in Services.CurrentStates.DeviceStates)
                {
                    if (deviceState.ChangeEntities.ParameterChanged)
                    {
                        currentStates.DeviceStates.Add(deviceState);
                    }
                }

                StateService.StatesChanged(currentStates);
            }
            catch (Exception e)
            {
                Trace.WriteLine("OnParametersChanged Error: " + e.ToString());
            }
        }

        public void OnStateChanged(string coreStateString, Firesec.CoreState.config coreState)
        {
            try
            {
                Trace.WriteLine("OnStateChanged");
                SetStates(coreState);
                PropogateStates();
                CalculateStates();
                CalculateZones();

                CurrentStates currentStates = new CurrentStates();
                currentStates.DeviceStates = new List<DeviceState>();
                currentStates.ZoneStates = new List<ZoneState>();

                foreach (DeviceState device in Services.CurrentStates.DeviceStates)
                {
                    if ((device.ChangeEntities.StatesChanged) || (device.ChangeEntities.StateChanged))
                    {
                        currentStates.DeviceStates.Add(device);
                    }
                }

                foreach (ZoneState zone in Services.CurrentStates.ZoneStates)
                {
                    if (zone.ZoneChanged)
                    {
                        currentStates.ZoneStates.Add(zone);
                    }
                }

                StateService.StatesChanged(currentStates);
                Trace.WriteLine("OnStateChanged End");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Watcher Error: " + e.ToString());
            }
        }

        void SetStates(Firesec.CoreState.config coreState)
        {
            foreach (DeviceState deviceState in Services.CurrentStates.DeviceStates)
            {
                deviceState.ChangeEntities.IsNewEvent = false;

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
            foreach (DeviceState deviceState in Services.CurrentStates.DeviceStates)
            {
                deviceState.ParentStates = new List<InnerState>();
                deviceState.ParentStringStates = new List<string>();
            }

            foreach (DeviceState deviceState in Services.CurrentStates.DeviceStates)
            {
                foreach (InnerState state in deviceState.InnerStates)
                {
                    if ((state.IsActive) && (state.AffectChildren))
                    {
                        foreach (DeviceState chilDevice in Services.CurrentStates.DeviceStates)
                        {
                            if ((chilDevice.PlaceInTree.StartsWith(deviceState.PlaceInTree)) && (chilDevice.PlaceInTree != deviceState.PlaceInTree))
                            {
                                chilDevice.ParentStates.Add(state);
                                string driverId = Services.AllDevices.FirstOrDefault(x => x.Path == deviceState.Path).DriverId;
                                string driverName = Services.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == driverId).shortName;
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
            foreach (DeviceState deviceState in Services.CurrentStates.DeviceStates)
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
                foreach (InnerState state in deviceState.ParentStates)
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
            if (Services.CurrentStates.ZoneStates != null)
            {
                foreach (ZoneState zoneState in Services.CurrentStates.ZoneStates)
                {
                    int minZonePriority = 7;
                    foreach (Device device in Services.AllDevices)
                    {
                        if (device.ZoneNo == zoneState.No)
                        {
                            DeviceState deviceState = Services.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == device.Path);
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
