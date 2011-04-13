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
        public static ServiceApi.CurrentConfiguration CurrentConfiguration { get; set; }
        public static ServiceApi.CurrentStates CurrentStates { get; set; }

        public void Start()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/StateService");
            duplexChannelFactory = new DuplexChannelFactory<IStateService>(new InstanceContext(this), binding, endpointAddress);
            stateService = duplexChannelFactory.CreateChannel();
            CurrentConfiguration = stateService.GetConfiguration();
            CurrentConfiguration.FillAllDevices();
            CurrentConfiguration.SetUnderlyingZones();
            stateService.Initialize();
            CurrentStates = stateService.GetStates();
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

        public void StateChanged(CurrentStates currentStates)
        {
            foreach (DeviceState deviceState in currentStates.DeviceStates)
            {
                DeviceState localDeviceState = CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == deviceState.Path);
                localDeviceState = deviceState;
                CurrentStates.OnDeviceStateChanged(deviceState);
            }
            foreach (ZoneState zoneState in currentStates.ZoneStates)
            {
                ZoneState localZoneState = CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneState.No);
                localZoneState = zoneState;
                CurrentStates.OnZoneStateChanged(zoneState);
            }
        }

        public void NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            CurrentStates.OnNewJournalEvent(journalItem);
        }

        public void SetNewConfig(CurrentConfiguration currentConfiguration)
        {
            stateService.SetConfiguration(currentConfiguration);
        }

        public void ResetState(Device device, string command)
        {
            stateService.ResetState(device.Path, command);
        }

        public List<Firesec.ReadEvents.journalType> ReadJournal(int startIndex, int count)
        {
            return stateService.ReadJournal(startIndex, count);
        }
    }
}
