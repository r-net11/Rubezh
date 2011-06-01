using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;

namespace DevicesModule.PropertyBindings
{
    public class BaseProperty : BaseViewModel
    {
        public BaseProperty(Firesec.Metadata.propInfoType propertyInfo, Device device)
        {
            _propertyInfo = propertyInfo;
            _device = device;
            Caption = propertyInfo.caption;
        }

        protected Device _device;
        protected Firesec.Metadata.propInfoType _propertyInfo;

        public string Caption { get; private set; }

        protected void Save(string Value)
        {
            if (_device.Properties == null)
                _device.Properties = new List<Property>();
            var property = _device.Properties.FirstOrDefault(x => x.Name == _propertyInfo.name);

            if (Value == _propertyInfo.@default)
            {
                if (property != null)
                {
                    _device.Properties.Remove(property);
                    return;
                }
            }

            if (property != null)
            {
                property.Name = _propertyInfo.name;
                property.Value = Value;
            }
            else
            {
                Property newProperty = new Property();
                newProperty.Name = _propertyInfo.name;
                newProperty.Value = Value;
                _device.Properties.Add(newProperty);
            }
        }
    }
}
