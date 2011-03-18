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
        public static ServiceApi.StateConfiguration StateConfiguration { get; private set; }
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
            SetParents();
            SetZones();
            SetStates();
        }

        void SetParents()
        {
            Configuration = new Configuration();
            Configuration.Devices = new List<Device>();

            ShortDevice rootShortDevice = StateConfiguration.RootShortDevice;
            Device rootFullDevice = new Device();
            rootFullDevice.SetConfig(rootShortDevice);
            rootFullDevice.Parent = null;
            Configuration.Devices.Add(rootFullDevice);
            AddMyDevice(rootShortDevice, rootFullDevice);    
        }

        void AddMyDevice(ShortDevice parentShortDevice, Device parentFullDevice)
        {
            parentFullDevice.Children = new List<Device>();
            foreach (ShortDevice shortDevice in parentShortDevice.Children)
            {
                Device fullDevice = new Device();
                fullDevice.SetConfig(shortDevice);
                fullDevice.Parent = parentFullDevice;
                parentFullDevice.Children.Add(fullDevice);
                Configuration.Devices.Add(fullDevice);
                AddMyDevice(shortDevice, fullDevice);
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
                OnDeviceStateChanged(shortDeviceState);
            }
            foreach (ShortZoneState shortZoneState in shortStates.ShortZoneStates)
            {
                // OnZoneChanged
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

        public static event Action<ShortDeviceState> DeviceStateChanged;
        static void OnDeviceStateChanged(ShortDeviceState shortDeviceState)
        {
            if (DeviceStateChanged != null)
            {
                DeviceStateChanged(shortDeviceState);
            }
        }
    }
}
