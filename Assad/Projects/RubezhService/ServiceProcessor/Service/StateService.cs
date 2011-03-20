using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using ServiseProcessor;
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

        public static void StatesChanged(ShortStates shortStates)
        {
            if (callback != null)
            {
                callback.StateChanged(shortStates);
            }
        }

        public StateConfiguration GetConfiguration()
        {
            Device rootDevice = Services.Configuration.Devices[0];
            ShortDevice rootShortDevice = rootDevice.ToShortDevice();
            AddShortDevice(rootDevice, rootShortDevice);
            Services.StateConfiguration = new StateConfiguration();
            Services.StateConfiguration.RootShortDevice = rootShortDevice;

            Services.StateConfiguration.ShortZones = new List<ShortZone>();
            foreach (Zone zone in Services.Configuration.Zones)
            {
                ShortZone shortZone = zone.ToShortZone();
                Services.StateConfiguration.ShortZones.Add(shortZone);
            }

            Services.StateConfiguration.Metadata = Services.Configuration.Metadata;
            return Services.StateConfiguration;
        }

        void AddShortDevice(Device parentDevice, ShortDevice parentShortDevice)
        {
            parentShortDevice.Children = new List<ShortDevice>();
            foreach (Device device in parentDevice.Children)
            {
                ShortDevice shortDevice = device.ToShortDevice();
                parentShortDevice.Children.Add(shortDevice);
                AddShortDevice(device, shortDevice);
            }
        }

        public ShortStates GetStates()
        {
            ShortStates shortStates = new ShortStates();
            shortStates.ShortDeviceStates = new List<ShortDeviceState>();
            shortStates.ShortZoneStates = new List<ShortZoneState>();

            foreach (Device device in Services.Configuration.Devices)
            {
                shortStates.ShortDeviceStates.Add(device.ToShortDeviceState());
            }

            foreach (Zone zone in Services.Configuration.Zones)
            {
                shortStates.ShortZoneStates.Add(zone.ToShortZoneState());
            }

            return shortStates;
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

        public void SetConfiguration(StateConfiguration data)
        {
            //Processor.SetNewConfig(data);
        }
    }
}
