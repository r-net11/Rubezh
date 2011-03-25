using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ServiceApi;

namespace ClientApi
{
    public class ServiceClient : ICallback
    {
        DuplexChannelFactory<IStateService> duplexChannelFactory;
        IStateService stateService;
        static ServiceApi.StateConfiguration StateConfiguration { get; set; }
        public static Configuration Configuration { get; private set; }

        public void Start()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/StateService");
            duplexChannelFactory = new DuplexChannelFactory<IStateService>(new InstanceContext(this), binding, endpointAddress);
            stateService = duplexChannelFactory.CreateChannel();
            StateConfiguration = stateService.GetConfiguration();
            stateService.Initialize();
            Configuration = new Configuration();
            Configuration.Metadata = StateConfiguration.Metadata;
            SetDevices();
            SetZones();
            SetStates();
            SetDeviceZones();
        }

        void SetDeviceZones()
        {
            foreach (Device device in Configuration.Devices)
            {
                device.SetZone();
            }
        }

        void SetDevices()
        {
            Configuration.Devices = new List<Device>();

            ShortDevice rootShortDevice = StateConfiguration.RootShortDevice;
            Device rootDevice = new Device();
            rootDevice.SetConfig(rootShortDevice);
            rootDevice.Parent = null;
            Configuration.Devices.Add(rootDevice);
            AddDevice(rootShortDevice, rootDevice);    
        }

        void AddDevice(ShortDevice parentShortDevice, Device parentDevice)
        {
            parentDevice.Children = new List<Device>();
            foreach (ShortDevice shortDevice in parentShortDevice.Children)
            {
                Device device = new Device();
                device.SetConfig(shortDevice);
                device.Parent = parentDevice;
                parentDevice.Children.Add(device);
                Configuration.Devices.Add(device);
                AddDevice(shortDevice, device);
            }
        }

        void SetZones()
        {
            // добавить ссылки на устройства
            Configuration.Zones = new List<Zone>();
            foreach (ShortZone shortZone in StateConfiguration.ShortZones)
            {
                Zone fullZone = new Zone();
                fullZone.SetConfig(shortZone);
                Configuration.Zones.Add(fullZone);
            }
        }

        void SetStates()
        {
            ShortStates shortStates = stateService.GetStates();

            foreach (ShortDeviceState shortDeviceState in shortStates.ShortDeviceStates)
            {
                Device device = Configuration.Devices.FirstOrDefault(x => x.Path == shortDeviceState.Path);
                device.SetState(shortDeviceState);
            }

            foreach (ShortZoneState shortZoneState in shortStates.ShortZoneStates)
            {
                Zone zone = Configuration.Zones.FirstOrDefault(x => x.Id == shortZoneState.Id);
                zone.SetState(shortZoneState);
            }
        }

        public void Stop()
        {
            //duplexChannelFactory.Close();
        }

        public void Notify(string message)
        {
        }

        public void ConfigurationChanged()
        {
        }

        public void StateChanged(ShortStates shortStates)
        {
            foreach (ShortDeviceState shortDeviceState in shortStates.ShortDeviceStates)
            {
                Device device = Configuration.Devices.FirstOrDefault(x => x.Path == shortDeviceState.Path);
                device.SetState(shortDeviceState);
            }

            foreach (ShortZoneState shortZoneState in shortStates.ShortZoneStates)
            {
                Zone zone = Configuration.Zones.FirstOrDefault(x => x.Id == shortZoneState.Id);
                zone.SetState(shortZoneState);
            }

            foreach (ShortDeviceState shortDeviceState in shortStates.ShortDeviceStates)
            {
                Device device = Configuration.Devices.FirstOrDefault(x => x.Path == shortDeviceState.Path);
                Configuration.OnDeviceStateChanged(device);
            }
            foreach (ShortZoneState shortZoneState in shortStates.ShortZoneStates)
            {
                Zone zone = Configuration.Zones.FirstOrDefault(x => x.Id == shortZoneState.Id);
                Configuration.OnZoneStateChanged(zone);
            }
        }

        public void SetNewConfig(StateConfiguration configuration)
        {
            //stateService.SetConfiguration(configuration);
        }

        public void ExecuteCommand(Device device, string command)
        {
            stateService.ExecuteCommand(device.Path, command);
        }
    }
}
