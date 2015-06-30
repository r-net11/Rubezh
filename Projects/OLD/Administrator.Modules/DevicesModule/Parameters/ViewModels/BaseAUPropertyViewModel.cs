using System.Linq;
using DevicesModule.ViewModels;
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

			UpdateDeviceParameterMissmatchType();
		}

		public string Caption
		{
			get { return DriverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return DriverProperty.ToolTip; }
		}

		void UpdateDeviceParameterMissmatchType()
		{
			var deviceProperty = Device.DeviceAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			var systemProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
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
				OnPropertyChanged(() => DeviceParameterMissmatchType);
			}
		}

		public virtual string DeviceAUParameterValue { get; protected set; }

		public bool IsEnabled
		{
			get { return !DriverProperty.IsReadOnly; }
		}

		protected void Save(string value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.FSParametersChanged = true;
			}

			var systemProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.Value = value;
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
			UpdateDeviceParameterMissmatchType();
			Device.OnChanged();
		}
	}
}