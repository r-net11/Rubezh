using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class PropertiesViewModel : BaseViewModel
	{
		public GKDevice Device { get; private set; }
		public GKDirection GKDirection { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }
		public bool HasAUParameters { get; private set; }

		public PropertiesViewModel(GKDevice device)
		{
			Device = device;
			Update();
			device.AUParametersChanged += On_AUParametersChanged;
		}
		public void Update()
		{
			StringProperties = new List<StringPropertyViewModel>();
			ShortProperties = new List<ShortPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			if (Device != null)
			{
				if (Device.PredefinedName == "Тест")
				{
					return;
				}
				foreach (var driverProperty in Device.Driver.Properties)
				{
					if (driverProperty.CanNotEdit)
						continue;

					switch (driverProperty.DriverPropertyType)
					{
						case GKDriverPropertyTypeEnum.StringType:
							StringProperties.Add(new StringPropertyViewModel(driverProperty, Device));
							break;
						case GKDriverPropertyTypeEnum.IntType:
							ShortProperties.Add(new ShortPropertyViewModel(driverProperty, Device));
							break;
						case GKDriverPropertyTypeEnum.BoolType:
							BoolProperties.Add(new BoolPropertyViewModel(driverProperty, Device));
							break;
						case GKDriverPropertyTypeEnum.EnumType:
							EnumProperties.Add(new EnumPropertyViewModel(driverProperty, Device));
							break;
					}

					HasAUParameters = true;
				}
			}

			OnPropertyChanged(() => StringProperties);
			OnPropertyChanged(() => ShortProperties);
			OnPropertyChanged(() => BoolProperties);
			OnPropertyChanged(() => EnumProperties);
			UpdateDeviceParameterMissmatchType();
		}

		public void UpdateDeviceParameterMissmatchType()
		{
			if (StringProperties.Any() || ShortProperties.Any() || BoolProperties.Any() || EnumProperties.Any())
			{
				var maxDeviceParameterMissmatchType = DeviceParameterMissmatchType.Equal;
				foreach (var auProperty in StringProperties)
				{
					if (auProperty.IsAUParameter)
					{
						if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
							maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
					}
				}
				foreach (var auProperty in ShortProperties)
				{
					if (auProperty.IsAUParameter)
					{
						if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
							maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
					}
				}
				foreach (var auProperty in BoolProperties)
				{
					if (auProperty.IsAUParameter)
					{
						if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
							maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
					}
				}
				foreach (var auProperty in EnumProperties)
				{
					if (auProperty.IsAUParameter)
					{
						if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
							maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
					}
				}
				DeviceParameterMissmatchType = maxDeviceParameterMissmatchType;
			}
		}

		DeviceParameterMissmatchType _deviceParameterMissmatchType;
		public DeviceParameterMissmatchType DeviceParameterMissmatchType
		{
			get{return _deviceParameterMissmatchType;}
			set
			{
				_deviceParameterMissmatchType = value;
				OnPropertyChanged(() => DeviceParameterMissmatchType);
			}
		}

		public bool HasParameters
		{
			get
			{
				if ((Device == null) || (Device.PredefinedName == "Тест"))
					return false;
				return Device.Properties.Count != 0;
			}
		}

		void On_AUParametersChanged()
		{
			Update();
		}
	}
}