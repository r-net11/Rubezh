using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.DeviceProperties
{
    public class BasePropertyViewModel : BaseViewModel
    {
        public BasePropertyViewModel(DriverProperty driverProperty, Device device)
        {
            _driverProperty = driverProperty;
            _device = device;
            Caption = driverProperty.Caption;
        }

        protected Device _device;
        protected DriverProperty _driverProperty;

        public string Caption { get; private set; }

        protected void Save(string Value)
        {
            if (_device.Properties == null)
                _device.Properties = new List<Property>();
            var property = _device.Properties.FirstOrDefault(x => x.Name == _driverProperty.Name);

            if (Value == _driverProperty.Default)
            {
                if (property != null)
                {
                    _device.Properties.Remove(property);
                    return;
                }
            }

            if (property != null)
            {
                property.Name = _driverProperty.Name;
                property.Value = Value;
            }
            else
            {
                var newProperty = new Property();
                newProperty.Name = _driverProperty.Name;
                newProperty.Value = Value;
                _device.Properties.Add(newProperty);
            }
        }
    }
}