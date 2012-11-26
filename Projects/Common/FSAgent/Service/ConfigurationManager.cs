using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Common;
using System.Threading;
using Infrastructure.Common;
using System.ServiceModel;
using FiresecAPI;
using System.ServiceModel.Description;

namespace FSAgent.Service
{
    public static class ConfigurationManager
    {
        public static DeviceConfiguration DeviceConfiguration { get; private set; }
        public static DriversConfiguration DriversConfiguration { get; private set; }

        public static void GetConfiruration()
        {
            ChannelFactory<IFiresecService> channelFactory = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var serverAddress = "net.pipe://127.0.0.1/FiresecService/";
                    var binding = BindingHelper.CreateNetNamedPipeBinding();

                    var endpointAddress = new EndpointAddress(new Uri(serverAddress));
                    channelFactory = new ChannelFactory<IFiresecService>(binding, endpointAddress);

                    foreach (OperationDescription operationDescription in channelFactory.Endpoint.Contract.Operations)
                    {
                        DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
                        if (dataContractSerializerOperationBehavior != null)
                            dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
                    }

                    channelFactory.Open();

                    IFiresecService firesecService = channelFactory.CreateChannel();

                    DeviceConfiguration = firesecService.GetDeviceConfiguration();
                    DriversConfiguration = firesecService.GetDriversConfiguration();
                    UpdateConfiguration();
                    CreateStates();

                    channelFactory.Close();
                }
                catch (Exception e)
                {
                    Logger.Error(e, "ConfigurationManager.GetConfiruration");
                    ServerLoadHelper.Reload();
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
        }

        static void SetConfiguration(DeviceConfiguration deviceConfiguration, DriversConfiguration driversConfiguration)
        {
            DeviceConfiguration = deviceConfiguration;
            DriversConfiguration = driversConfiguration;
            UpdateConfiguration();
            CreateStates();
        }

        static void UpdateConfiguration()
        {
            if (DeviceConfiguration == null)
            {
                return;
            }

            DeviceConfiguration.Update();
            DeviceConfiguration.Reorder();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
            }
            DeviceConfiguration.Devices.RemoveAll(x => x.Driver == null);

            DeviceConfiguration.InvalidateConfiguration();
            DeviceConfiguration.UpdateCrossReferences();
        }

        static void CreateStates()
        {
            foreach (var device in DeviceConfiguration.Devices)
            {
                var deviceState = new DeviceState()
                {
                    Device = device
                };
                foreach (var parameter in device.Driver.Parameters)
                    deviceState.ThreadSafeParameters.Add(parameter.Copy());
                device.DeviceState = deviceState;
            }

            foreach (var zone in DeviceConfiguration.Zones)
            {
                var zoneState = new ZoneState()
                {
                    Zone = zone,
                };
                zone.ZoneState = zoneState;
            }
        }
    }
}