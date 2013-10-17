using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class PropertiesViewModel : BaseViewModel
	{
		public XDevice Device { get; private set; }
		public XDirection XDirection { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }
		public bool HasAUParameters { get; private set; }

		public PropertiesViewModel(XDevice device)
		{
			Device = device;
			Update();
		}
		public void Update()
		{
			StringProperties = new List<StringPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			if (Device != null)
			{
				foreach (var driverProperty in Device.Driver.Properties)
				{
					if (driverProperty.IsAUParameter)
					{
						switch (driverProperty.DriverPropertyType)
						{
							case XDriverPropertyTypeEnum.EnumType:
								EnumProperties.Add(new EnumPropertyViewModel(driverProperty, Device));
								break;

							case XDriverPropertyTypeEnum.StringType:
							case XDriverPropertyTypeEnum.IntType:
							case XDriverPropertyTypeEnum.BoolType:
								StringProperties.Add(new StringPropertyViewModel(driverProperty, Device));
								break;
						}

						HasAUParameters = true;
					}
				}
			}

			OnPropertyChanged("StringProperties");
			OnPropertyChanged("EnumProperties");
			UpdateDeviceParameterMissmatchType();
		}

		public void UpdateDeviceParameterMissmatchType()
		{
			if (StringProperties.Count + EnumProperties.Count > 0)
			{
				DeviceParameterMissmatchType maxDeviceParameterMissmatchType = DeviceParameterMissmatchType.Equal;
				foreach (var auProperty in StringProperties)
				{
					if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
						maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
				}
				foreach (var auProperty in EnumProperties)
				{
					if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
						maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
				}
				DeviceParameterMissmatchType = maxDeviceParameterMissmatchType;
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
	}
}