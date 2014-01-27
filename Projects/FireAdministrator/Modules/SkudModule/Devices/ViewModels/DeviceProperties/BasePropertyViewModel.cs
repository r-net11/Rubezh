using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected SKDDevice Device;
		protected XDriverProperty DriverProperty;

		public BasePropertyViewModel(XDriverProperty driverProperty, SKDDevice device)
		{
			DriverProperty = driverProperty;
            IsAUParameter = driverProperty.IsAUParameter;
			Device = device;

			if (!Device.Properties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}

			if (Device.DeviceProperties == null)
			{
				Device.DeviceProperties = new List<XProperty>();
			}

			var deviceProperty = Device.DeviceProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (deviceProperty != null)
			{
				DeviceAUParameterValue = deviceProperty.Value.ToString();
			}
			else
				DeviceAUParameterValue = "Неизвестно";

			UpdateDeviceParameterMissmatchType();
		}

        public bool IsAUParameter { get; set; }
		public string Caption
		{
			get { return DriverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return DriverProperty.ToolTip; }
		}

		protected void UpdateDeviceParameterMissmatchType()
		{
			var deviceProperty = Device.DeviceProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (!DriverProperty.IsReadOnly)
			{
				if (deviceProperty == null)
				{
					DeviceParameterMissmatchType = DeviceParameterMissmatchType.Unknown;
				}
				else
				{
                    if (systemProperty != null && deviceProperty.Value == systemProperty.Value)
						DeviceParameterMissmatchType = DeviceParameterMissmatchType.Equal;
					else
						DeviceParameterMissmatchType = DeviceParameterMissmatchType.Unequal;
				}
			}
		}

		DeviceParameterMissmatchType _deviceParameterMissmatchType;
		public DeviceParameterMissmatchType DeviceParameterMissmatchType
		{
			get { return _deviceParameterMissmatchType; }
			set
			{
				_deviceParameterMissmatchType = value;
				OnPropertyChanged("DeviceParameterMissmatchType");
			}
		}

		public virtual string DeviceAUParameterValue { get; protected set; }

		public bool IsEnabled
		{
			get { return !DriverProperty.IsReadOnly; }
		}

		protected void Save(ushort value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}

			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.Value = value;
			}
			else
			{
				var newProperty = new XProperty()
				{
					Name = DriverProperty.Name,
					Value = value,
				};
				Device.Properties.Add(newProperty);
			}
			UpdateDeviceParameterMissmatchType();
			Device.OnChanged();
		}
	}
}