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
                callback.DeviceChanged(device);
            }
        }

        public static void ZoneChanged(Zone zone)
        {
            if (callback != null)
            {
                callback.ZoneChanged(zone);
            }
        }

        public Configuration GetConfiguration()
        {
            Device rootDevice = Services.Configuration.Devices[0];
            ShortDevice rootShortDevice = new ShortDevice();
            rootShortDevice.Name = rootDevice.DriverName;
            rootShortDevice.Parent = null;
            AddShortDevice(rootDevice, rootShortDevice);

            Services.Configuration.shortDevice = rootShortDevice;
            return Services.Configuration;
        }

        void AddShortDevice(Device parentDevice, ShortDevice parentShortDevice)
        {
            parentShortDevice.Children = new List<ShortDevice>();
            foreach (Device device in parentDevice.Children)
            {
                ShortDevice shortDevice = new ShortDevice();
                shortDevice.Name = device.DriverName;
                shortDevice.Parent = parentShortDevice;
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
