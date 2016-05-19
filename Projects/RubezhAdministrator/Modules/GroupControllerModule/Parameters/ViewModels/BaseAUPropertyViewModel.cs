using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.DeviceProperties
{
	public class BaseAUPropertyViewModel : BaseViewModel
	{
		protected GKDevice Device;
		protected GKDriverProperty DriverProperty;

		public BaseAUPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
		{
			DriverProperty = driverProperty;
			Device = device;

			if (!Device.Properties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}

			if (Device.DeviceProperties == null)
			{
				Device.DeviceProperties = new List<GKProperty>();
			}
			
			var deviceProperty = Device.DeviceProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (deviceProperty != null)
			{
				DeviceAUParameterValue = deviceProperty.Value.ToString();
				//if ((deviceProperty.DriverProperty != null) && (deviceProperty.DriverProperty.DriverPropertyType == XDriverPropertyTypeEnum.EnumType))
					//DeviceAUParameterValue = deviceProperty.DriverProperty.Parameters.FirstOrDefault(x => x.Value == deviceProperty.Value).Name;
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
				OnPropertyChanged(() => DeviceParameterMissmatchType);
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
				ServiceFactory.SaveService.GKChanged = true;
			}

			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.Value = value;
			}
			else
			{
				var newProperty = new GKProperty()
				{
					Name = DriverProperty.Name,
					Value = value
				};
				Device.Properties.Add(newProperty);
			}
			UpdateDeviceParameterMissmatchType();
			Device.OnChanged();
		}
	}
}