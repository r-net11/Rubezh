using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiseProcessor;
using Common;
using System.Diagnostics;
using ServiceApi;

namespace ServiseProcessor
{
    public class Watcher
    {
        internal void Start()
        {
            Firesec.FiresecEventAggregator.NewEvent += new Action<string, Firesec.CoreState.config>(OnComServerStateChanged);
        }

        public void OnComServerStateChanged(string coreStateString, Firesec.CoreState.config coreState)
        {
            try
            {
                List<DeviceEvent> DeviceEvents = new List<DeviceEvent>();

                foreach (Device device in Services.Configuration.Devices)
                {
                    DeviceEvent deviceEvent = new DeviceEvent();
                    deviceEvent.device = device;
                    deviceEvent.events = new List<string>();

                    Firesec.CoreState.devType innerDevice = FindDevice(coreState.dev, device.PlaceInTree);

                    if ((innerDevice != null) && (innerDevice.state != null))
                    {
                        foreach (State state in device.States)
                        {
                            bool IsActive = innerDevice.state.Any(a => a.id == state.Id);

                            if ((state.IsActive == false) && (IsActive == true))
                            {
                                deviceEvent.events.Add(state.Name);
                            }
                            if ((state.IsActive == true) && (IsActive == false))
                            {
                                deviceEvent.events.Add("Сброс " + state.Name);
                            }

                            state.IsActive = IsActive;
                        }
                    }
                    else
                    {
                        bool hasOneActiveState = false;
                        foreach (State state in device.States)
                        {
                            if (state.IsActive)
                            {
                                hasOneActiveState = true;
                            }
                            state.IsActive = false;
                        }
                        if (hasOneActiveState)
                        {
                            deviceEvent.events.Add("Норма");
                        }
                    }

                    if (deviceEvent.events.Count > 0)
                    {
                        DeviceEvents.Add(deviceEvent);
                    }

                    // выставить нужное состояние
                    int minPriority = 7;
                    State sourceState = null;

                    foreach (State state in device.States)
                    {
                        if (state.IsActive)
                        {
                            if (state.Priority < minPriority)
                            {
                                minPriority = state.Priority;
                                sourceState = state;
                            }
                        }
                    }
                    device.State = StateHelper.GetState(minPriority);
                    device.MinPriority = minPriority;

                    if (sourceState != null)
                    {
                        device.SourceState = sourceState.Name;
                        if (sourceState.AffectChildren)
                            device.AffectChildren = true;
                    }
                    else
                    {
                        device.SourceState = "";
                        device.AffectChildren = false;
                    }
                }

                foreach (Device device in Services.Configuration.Devices)
                {
                    if (device.AffectChildren)
                    {
                        List<Device> childDevices = Services.Configuration.Devices.FindAll(x=>((x.PlaceInTree.StartsWith(device.PlaceInTree)) && x.PlaceInTree != device.PlaceInTree));
                        foreach (Device childDevice in childDevices)
                        {
                            childDevice.MinPriority = device.MinPriority;
                            childDevice.State = device.State;
                        }
                    }
                }

                OnNewDeviceEvent(DeviceEvents);
                foreach (DeviceEvent deviceEvent in DeviceEvents)
                {
                    Console.WriteLine("NEW EVENT");
                    deviceEvent.device.LastEvents = deviceEvent.events;
                    StateService.DeviceChanged(deviceEvent.device);
                }

                // изменить состояние зоны

                if (Services.Configuration.Zones != null)
                {
                    foreach (Zone zone in Services.Configuration.Zones)
                    {
                        int minZonePriority = 7;
                        if (zone.Devices != null)
                        {
                            foreach (Device zoneDevice in zone.Devices)
                            {
                                if (zoneDevice.MinPriority < minZonePriority)
                                    minZonePriority = zoneDevice.MinPriority;
                            }
                        }
                        string newZoneState = StateHelper.GetState(minZonePriority);

                        bool isZoneChanged = false;
                        if ((zone.State == null) || (zone.State != newZoneState))
                        {
                            isZoneChanged = true;
                        }
                        zone.State = newZoneState;
                        if (isZoneChanged)
                        {
                            StateService.ZoneChanged(zone);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("EXCEPTION IN OnComServerStateChanged: " + ex.ToString());
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

        public static event Action<List<DeviceEvent>> NewDeviceEvent;
        public static void OnNewDeviceEvent(List<DeviceEvent> DeviceEvents)
        {
            if (NewDeviceEvent != null)
                NewDeviceEvent(DeviceEvents);
        }
    }

    public class DeviceEvent
    {
        public Device device { get; set; }
        public List<string> events { get; set; }
    }
}
