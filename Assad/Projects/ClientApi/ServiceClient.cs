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
        public static Configuration Configuration { get; private set; }

        public void Start()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = 10000000;
            binding.MaxReceivedMessageSize = 10000000;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/StateService");
            duplexChannelFactory = new DuplexChannelFactory<IStateService>(new InstanceContext(this), binding, endpointAddress);
            stateService = duplexChannelFactory.CreateChannel();
            Configuration = stateService.GetConfiguration();
            stateService.Initialize();
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

        public void SetNewConfig(Configuration configuration)
        {
            stateService.SetConfiguration(configuration);
        }

        public void ExecuteCommand(Device device, string command)
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
