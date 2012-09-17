using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.DeviceProperties
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected Device _device;
		protected DriverProperty _driverProperty;

		public BasePropertyViewModel(DriverProperty driverProperty, Device device)
		{
			_driverProperty = driverProperty;
			_device = device;
			if (_device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name)==null)
				Save(driverProperty.Default);
		}

		public string Caption
		{
			get { return _driverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return _driverProperty.ToolTip; }
		}

		public bool IsAUParameter
		{
			get { return _driverProperty.IsAUParameter; }
		}
        public bool IsControl
        {
            get { return _driverProperty.IsControl; }
        }

		protected void Save(string value)
		{
			ServiceFactory.SaveService.DevicesChanged = true;

			if (_device.Properties == null)
				_device.Properties = new List<Property>();
			var property = _device.Properties.FirstOrDefault(x => x.Name == _driverProperty.Name);

			//if (value == _driverProperty.Default)
			//{
			//    if (property != null)
			//    {
			//        _device.Properties.Remove(property);
			//        return;
			//    }
			//}

			if (property != null)
			{
				property.Name = _driverProperty.Name;
				property.Value = value;
			}
			else
			{
				var newProperty = new Property()
				{
					Name = _driverProperty.Name,
					Value = value
				};
				_device.Properties.Add(newProperty);
			}
		}
	}
}