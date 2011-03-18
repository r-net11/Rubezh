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
        public static ServiceApi.Configuration Configuration { get; private set; }
        public static Configuration _Configuration { get; private set; }

        public void Start()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/StateService");
            duplexChannelFactory = new DuplexChannelFactory<IStateService>(new InstanceContext(this), binding, endpointAddress);
            stateService = duplexChannelFactory.CreateChannel();
            Configuration = stateService.GetConfiguration();
            stateService.Initialize();
            SetParents();
            SetZones();
        }

        void SetParents()
        {
            _Configuration = new Configuration();
            _Configuration.Devices = new List<FullDevice>();

            ShortDevice rootShortDevice = Configuration.shortDevice;
            FullDevice rootFullDevice = new FullDevice(rootShortDevice);
            rootFullDevice.Parent = null;
            _Configuration.Devices.Add(rootFullDevice);
            AddMyDevice(rootShortDevice, rootFullDevice);    
        }

        void AddMyDevice(ShortDevice parentShortDevice, FullDevice parentFullDevice)
        {
            parentFullDevice.Children = new List<FullDevice>();
            foreach (ShortDevice shortDevice in parentShortDevice.Children)
            {
                FullDevice fullDevice = new FullDevice(shortDevice);
                fullDevice.Parent = parentFullDevice;
                parentFullDevice.Children.Add(fullDevice);
                _Configuration.Devices.Add(fullDevice);
                AddMyDevice(shortDevice, fullDevice);
            }
        }

        void SetZones()
        {
            // добавить ссылки на устройства
            _Configuration.Zones = new List<FullZone>();
            foreach (ShortZone shortZone in Configuration.ShortZones)
            {
                FullZone fullZone = new FullZone(shortZone);
                _Configuration.Zones.Add(fullZone);
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

        public void DeviceChanged(Device device)
        {
            OnDeviceStateChanged(device);
        }

        public void ZoneChanged(Zone zone)
        {
        }

        public void SetNewConfig(ServiceApi.Configuration configuration)
        {
            stateService.SetConfiguration(configuration);
        }

        public void ExecuteCommand(FullDevice device, string command)
        {
            stateService.ExecuteCommand(device.Path, command);
        }

        public static event Action<Device> DeviceStateChanged;
        static void OnDeviceStateChanged(Device device)
        {
            if (DeviceStateChanged != null)
            {
                DeviceStateChanged(device);
            }
        }
    }
}
