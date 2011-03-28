using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientApi
{
    public class Device
    {
        public Device Parent { get; set; }
        public List<Device> Children { get; set; }

        public string Path { get; set; }
        public string DriverId { get; set; }
        public string Description { get; set; }
        public string PlaceInTree { get; set; }
        public string Address { get; set; }
        public string PresentationAddress { get; set; }
        public string Zone { get; set; }
        public Zone _Zone { get; set; }
        public List<ServiceApi.DeviceProperty> DeviceProperties { get; set; }
        public List<string> AvailableFunctions { get; set; }
        public string ValidationError { get; set; }

        public string State { get; set; }
        public List<string> States { get; set; }
        public List<Parameter> Parameters { get; set; }
        public bool StateChanged { get; set; }
        public bool StatesChanged { get; set; }
        public bool ParameterChanged { get; set; }
        public bool VisibleParameterChanged { get; set; }

        public void SetConfig(ServiceApi.ShortDevice shortDevice)
        {
            this.DriverId = shortDevice.DriverId;
            this.Address = shortDevice.Address;
            this.PresentationAddress = shortDevice.PresentationAddress;
            this.PlaceInTree = shortDevice.PlaceInTree;
            this.Path = shortDevice.Path;
            this.Description = shortDevice.Description;
            this.Zone = shortDevice.Zone;

            this.Parameters = new List<Parameter>();
            if (shortDevice.Parameters != null)
            {
                foreach (ServiceApi.Parameter parameter in shortDevice.Parameters)
                {
                    Parameter newParameter = new Parameter();
                    newParameter.Name = parameter.Name;
                    newParameter.Caption = parameter.Caption;
                    newParameter.Visible = parameter.Visible;
                    newParameter.Value = parameter.Value;
                    this.Parameters.Add(newParameter);
                }
            }

            DeviceProperties = new List<ServiceApi.DeviceProperty>();
            foreach (ServiceApi.DeviceProperty shortDeviceProperty in shortDevice.DeviceProperties)
            {
                ServiceApi.DeviceProperty deviceProperty = new ServiceApi.DeviceProperty();
                deviceProperty.Name = shortDeviceProperty.Name;
                deviceProperty.Value = shortDeviceProperty.Value;
                DeviceProperties.Add(deviceProperty);
            }

            Firesec.Metadata.drvType driver = ServiceClient.Configuration.Metadata.drv.FirstOrDefault(x => x.id == DriverId);
        }

        public void SetZone()
        {
            if (Zone != null)
            {
                if (ServiceClient.Configuration.Zones.Any(x => x.Id == Zone))
                {
                    _Zone = ServiceClient.Configuration.Zones.FirstOrDefault(x => x.Id == Zone);
                }
            }
        }

        public void SetState(ServiceApi.ShortDeviceState shortDeviceState)
        {
            this.State = shortDeviceState.State;
            this.States = shortDeviceState.States;

            this.Parameters = new List<Parameter>();
            foreach (ServiceApi.Parameter parameter in shortDeviceState.Parameters)
            {
                Parameter newParameter = new Parameter();
                newParameter.Name = parameter.Name;
                newParameter.Caption = parameter.Caption;
                newParameter.Visible = parameter.Visible;
                newParameter.Value = parameter.Value;
                this.Parameters.Add(newParameter);
            }

            this.StateChanged = shortDeviceState.StateChanged;
            this.StatesChanged = shortDeviceState.StatesChanged;
            this.ParameterChanged = shortDeviceState.ParameterChanged;
            this.VisibleParameterChanged = shortDeviceState.VisibleParameterChanged;
        }
    }
}
