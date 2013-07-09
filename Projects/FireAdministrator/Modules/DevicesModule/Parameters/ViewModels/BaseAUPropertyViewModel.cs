using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.DeviceProperties
{
	public class BaseAUPropertyViewModel : BaseViewModel
	{
		protected Device Device;
		protected DriverProperty DriverProperty;

		public BaseAUPropertyViewModel(DriverProperty driverProperty, Device device)
		{
			DriverProperty = driverProperty;
			Device = device;
			if (Device.DeviceAUProperties != null)
				Device.DeviceAUProperties = new List<Property>();

			if (!Device.SystemAUProperties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}
			var deviceProperty = Device.DeviceAUProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (deviceProperty != null)
			{
				DeviceAUParameterValue = deviceProperty.Value;
			}
			else
				DeviceAUParameterValue = "Неизвестно";

			var systemProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null && deviceProperty != null)
			{
				IsMissmatch = systemProperty.Value != deviceProperty.Value;
			}
		}

		public string Caption
		{
			get { return DriverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return DriverProperty.ToolTip; }
		}

		public bool IsMissmatch { get; set; }
		public virtual string DeviceAUParameterValue { get; protected set; }

		protected void Save(string value, bool useSaveService = true)
		{
            if (useSaveService)
            {
                ServiceFactory.SaveService.FSChanged = true;
            }

			var property = Device.SystemAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (property != null)
			{
				property.Name = DriverProperty.Name;
				property.Value = value;
			}
			else
			{
				var newProperty = new Property()
				{
					Name = DriverProperty.Name,
					Value = value
				};
				Device.SystemAUProperties.Add(newProperty);
			}
			Device.OnChanged();
		}
	}
}