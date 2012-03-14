using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels
{
    public class BasePropertyViewModel : BaseViewModel
    {
        protected XDevice _xDevice;
        protected XDriverProperty _xDriverProperty;

        public BasePropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
        {
            _xDriverProperty = xDriverProperty;
            _xDevice = xDevice;
        }

        public string Caption
        {
            get { return _xDriverProperty.Caption; }
        }

        public string ToolTip
        {
            get { return _xDriverProperty.ToolTip; }
        }

        protected void Save(string value)
        {
            if (_xDevice.Properties == null)
                _xDevice.Properties = new List<XProperty>();
            var property = _xDevice.Properties.FirstOrDefault(x => x.Name == _xDriverProperty.Name);

            if (value == _xDriverProperty.Default)
            {
                if (property != null)
                {
                    _xDevice.Properties.Remove(property);
                    return;
                }
            }

            if (property != null)
            {
                property.Name = _xDriverProperty.Name;
                property.Value = value;
            }
            else
            {
                var newProperty = new XProperty()
                {
                    Name = _xDriverProperty.Name,
                    Value = value
                };
                _xDevice.Properties.Add(newProperty);
            }
        }
    }
}