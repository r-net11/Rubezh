using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using ServiseProcessor;
using Common;
using ServiceApi;

namespace ServiseProcessor
{
    class StateService : IStateService
    {
        static ICallback callback;

        public void Initialize()
        {
            callback = OperationContext.Current.GetCallbackChannel<ICallback>();
        }

        public static void Notify(string message)
        {
            if (callback != null)
                callback.Notify(message);
        }

        public static void DeviceChanged(Device device)
        {
            if (callback != null)
            {
                ShortDeviceState shortDeviceState = new ShortDeviceState();
                shortDeviceState.Path = device.Path;

                shortDeviceState.States = new List<string>();
                foreach (string state in device.SelfStates)
                {
                    shortDeviceState.States.Add(state);
                }
                foreach (string parentState in device.ParentStringStates)
                {
                    shortDeviceState.States.Add(parentState);
                }
                shortDeviceState.Parameters = new List<Parameter>();
                foreach (Parameter parameter in device.Parameters)
                {
                    shortDeviceState.Parameters.Add(new Parameter() { Caption = parameter.Caption, Value = parameter.Value });
                }
                shortDeviceState.MustUpdate = ((device.StateChanged) || (device.StatesChanged) || (device.VisibleParameterChanged));
                callback.DeviceChanged(shortDeviceState);
            }
        }

        public static void ZoneChanged(Zone zone)
        {
            if (callback != null)
            {
                ShortZoneState shortZoneState = new ShortZoneState();
                shortZoneState.State = zone.State;
                callback.ZoneChanged(shortZoneState);
            }
        }

        public Configuration GetConfiguration()
        {
            Device rootDevice = Services.Configuration.Devices[0];
            ShortDevice rootShortDevice = rootDevice.Copy();
            AddShortDevice(rootDevice, rootShortDevice);
            Services.Configuration.shortDevice = rootShortDevice;

            Services.Configuration.ShortZones = new List<ShortZone>();
            foreach (Zone zone in Services.Configuration.Zones)
            {
                ShortZone shortZone = zone.Copy();
                Services.Configuration.ShortZones.Add(shortZone);
            }

            Services.Configuration.Devices = null;
            Services.Configuration.Zones = null;
            return Services.Configuration;
        }

        void AddShortDevice(Device parentDevice, ShortDevice parentShortDevice)
        {
            parentShortDevice.Children = new List<ShortDevice>();
            foreach (Device device in parentDevice.Children)
            {
                ShortDevice shortDevice = device.Copy();
                parentShortDevice.Children.Add(shortDevice);
                AddShortDevice(device, shortDevice);
            }
        }

        public void ExecuteCommand(string devicePath, string command)
        {
            Device device;
            try
            {
                device = Services.Configuration.Devices.First(x => x.Path == devicePath);
            }
            catch
            {
                device = null;
            }
            if (device != null)
            {
                Processor.ExecuteCommand(device, command);
            }
        }

        public void SetConfiguration(Configuration data)
        {
            Processor.SetNewConfig(data);
        }
    }
}
