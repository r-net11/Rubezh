using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;

namespace ClientApi
{
    public class FullDevice
    {
        public FullDevice Parent { get; set; }

        public List<FullDevice> Children { get; set; }

        public FullDevice()
        {
        }

        public FullDevice(ShortDevice shortDevice)
        {
            this.DriverId = shortDevice.DriverId;
            this.Address = shortDevice.Address;
            this.PlaceInTree = shortDevice.PlaceInTree;
            this.Path = shortDevice.Path;
            this.Description = shortDevice.Description;
            this.Zone = shortDevice.Zone;

            if (shortDevice.Parameters != null)
            {
                this.Parameters = new List<Parameter>();
                foreach (Parameter parameter in shortDevice.Parameters)
                {
                    this.Parameters.Add(new Parameter()
                    {
                        Name = parameter.Name,
                        Caption = parameter.Caption,
                        Visible = parameter.Visible,
                        Value = parameter.Value
                    });
                }
            }
        }

        public string Description { get; set; }

        public string DriverId { get; set; }

        public string ValidationError { get; set; }

        public string Zone { get; set; }

        public List<DeviceProperty> DeviceProperties { get; set; }

        public List<Parameter> Parameters { get; set; }

        public List<string> AvailableFunctions { get; set; }

        public string State { get; set; }

        public List<string> States { get; set; }

        public string PlaceInTree { get; set; }

        public string Address { get; set; }

        public bool StateChanged { get; set; }

        public bool StatesChanged { get; set; }

        public bool ParameterChanged { get; set; }

        public bool VisibleParameterChanged { get; set; }

        public string Path { get; set; }
    }
}
