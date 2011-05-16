using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Firesec;
using System.IO;
using System.Xml.Serialization;

namespace FiresecClient
{
    public class Watcher
    {
        internal void Start()
        {
            FiresecInternalClient.NewEvent += new Action<int, string>(FiresecClient_NewEvent);

            OnStateChanged(FiresecInternalClient.firesecService.GetCoreState());
            OnParametersChanged(FiresecInternalClient.firesecService.GetCoreDeviceParams());
            SetLastEvent();
        }

        void FiresecClient_NewEvent(int EventMask, string obj)
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
                OnStateChanged(obj);
            }
            if (evmDeviceParamsUpdated)
            {
                OnParametersChanged(obj);
            }
            if (evmNewEvents)
            {
                OnNewEvent(obj);
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

        void OnNewEvent(string obj)
        {
            byte[] bytes = Encoding.Default.GetBytes(obj);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(Firesec.ReadEvents.document));
            Firesec.ReadEvents.document journal = (Firesec.ReadEvents.document)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            //Firesec.ReadEvents.document journal = FiresecClient.ReadEvents(0, 100);

            string journalString = FiresecInternalClient.JournalString;

            if ((journal != null) && (journal.Journal.Count() > 0))
            {
                foreach (Firesec.ReadEvents.journalType journalItem in journal.Journal)
                {
                    if (Convert.ToInt32(journalItem.IDEvents) > LastEventId)
                    {
                        CurrentStates.OnNewJournalEvent(journalItem);
                    }
                }
                LastEventId = Convert.ToInt32(journal.Journal[0].IDEvents);
            }
        }

        void OnParametersChanged(string obj)
        {
            byte[] bytes = Encoding.Default.GetBytes(obj);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(Firesec.DeviceParams.config));
            Firesec.DeviceParams.config coreParameters = (Firesec.DeviceParams.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            //Firesec.DeviceParams.config coreParameters = FiresecClient.GetDeviceParams();


            try
            {
                Trace.WriteLine("OnParametersChanged");

                foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
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

                        if (deviceState.ChangeEntities.ParameterChanged)
                        {
                            FiresecManager.CurrentStates.OnDeviceParametersChanged(deviceState.Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("OnParametersChanged Error: " + e.ToString());
            }
        }

        public void OnStateChanged(string obj)
        {
            byte[] bytes = Encoding.Default.GetBytes(obj);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreState.config));
            Firesec.CoreState.config coreState = (Firesec.CoreState.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            //Firesec.CoreState.config coreState = FiresecClient.GetCoreState();


            try
            {
                SetStates(coreState);
                PropogateStates();
                CalculateStates();
                CalculateZones();

                foreach (DeviceState device in FiresecManager.CurrentStates.DeviceStates)
                {
                    device.States = new List<string>();
                    foreach (string parentState in device.ParentStringStates)
                        device.States.Add(parentState);

                    foreach (string selfState in device.SelfStates)
                        device.States.Add(selfState);
                }

                foreach (DeviceState device in FiresecManager.CurrentStates.DeviceStates)
                {
                    if ((device.ChangeEntities.StatesChanged) || (device.ChangeEntities.StateChanged))
                    {
                        device.OnStateChanged();
                        FiresecManager.CurrentStates.OnDeviceStateChanged(device.Id);
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
            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
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
            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
            {
                deviceState.ParentInnerStates = new List<InnerState>();
                deviceState.ParentStringStates = new List<string>();
            }

            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
            {
                foreach (InnerState state in deviceState.InnerStates)
                {
                    if ((state.IsActive) && (state.AffectChildren))
                    {
                        foreach (DeviceState chilDevice in FiresecManager.CurrentStates.DeviceStates)
                        {
                            if ((chilDevice.PlaceInTree.StartsWith(deviceState.PlaceInTree)) && (chilDevice.PlaceInTree != deviceState.PlaceInTree))
                            {
                                chilDevice.ParentInnerStates.Add(state);
                                string driverId = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == deviceState.Id).DriverId;
                                string driverName = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == driverId).shortName;
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
            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
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
            if (FiresecManager.CurrentStates.ZoneStates != null)
            {
                foreach (ZoneState zoneState in FiresecManager.CurrentStates.ZoneStates)
                {
                    int minZonePriority = 7;
                    foreach (Device device in FiresecManager.CurrentConfiguration.AllDevices)
                    {
                        if (device.ZoneNo == zoneState.No)
                        {
                            DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == device.Id);
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
                        FiresecManager.CurrentStates.OnZoneStateChanged(zoneState.No);
                    }
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