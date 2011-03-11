﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComDevices;
using Common;
using System.Diagnostics;
using ServiceApi;

namespace ComDevices
{
    public class Watcher
    {
        internal void Start()
        {
            ComServer.ComEventAggregator.ComEvent += new Action<string, ComServer.CoreState.config>(OnComServerStateChanged);
        }

        public void OnComServerStateChanged(string coreStateString, ComServer.CoreState.config coreState)
        {
            try
            {
                List<DeviceEvent> DeviceEvents = new List<DeviceEvent>();

                foreach (Device device in Services.Configuration.Devices)
                {
                    DeviceEvent deviceEvent = new DeviceEvent();
                    deviceEvent.device = device;
                    deviceEvent.events = new List<string>();

                    ComServer.CoreState.devType innerDevice = FindDevice(coreState.dev, device.PlaceInTree);

                    if (innerDevice != null)
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
                    foreach (State state in device.States)
                    {
                        if (state.IsActive)
                        {
                            if (state.Priority < minPriority)
                                minPriority = state.Priority;
                        }
                    }
                    device.State = StateHelper.GetState(minPriority);
                }

                OnNewDeviceEvent(DeviceEvents);
                foreach (DeviceEvent deviceEvent in DeviceEvents)
                {
                    Console.WriteLine("NEW EVENT");
                    deviceEvent.device.LastEvents = deviceEvent.events;
                    StateService.DeviceChanged(deviceEvent.device);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("EXCEPTION IN OnComServerStateChanged: " + ex.ToString());
            }
        }

        ComServer.CoreState.devType FindDevice(ComServer.CoreState.devType[] innerDevices, string PlaceInTree)
        {
            ComServer.CoreState.devType innerDevice;
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
