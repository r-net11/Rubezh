using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.DeviceProperties;
using Infrastructure.Common.TreeList;

namespace GKModule.ViewModels
{
	public class DeviceParameterViewModel : TreeNodeViewModel<DeviceParameterViewModel>
	{
		public XDevice Device { get; private set; }
		public List<StringAUPropertyViewModel> StringAUProperties { get; set; }
		public List<EnumAUPropertyViewModel> EnumAUProperties { get; set; }
		public string PresentationZone { get; private set; }
		public bool HasAUParameters { get; private set; }

		public DeviceParameterViewModel(XDevice device)
		{
			Device = device;
			PresentationZone = XManager.GetPresentationZone(Device);
			Update();
			device.AUParametersChanged += new Action(On_AUParametersChanged);
		}
		
		public void Update()
		{
			StringAUProperties = new List<StringAUPropertyViewModel>();
			EnumAUProperties = new List<EnumAUPropertyViewModel>();
			if (Device != null)
			{
				foreach (var driverProperty in Device.Driver.Properties)
				{
					if (driverProperty.IsAUParameter)
					{
						switch (driverProperty.DriverPropertyType)
						{
							case XDriverPropertyTypeEnum.EnumType:
								EnumAUProperties.Add(new EnumAUPropertyViewModel(driverProperty, Device));
								break;

							case XDriverPropertyTypeEnum.StringType:
							case XDriverPropertyTypeEnum.IntType:
							case XDriverPropertyTypeEnum.BoolType:
								StringAUProperties.Add(new StringAUPropertyViewModel(driverProperty, Device));
								break;
						}

						HasAUParameters = true;
					}
				}
			}

			OnPropertyChanged(() => StringAUProperties);
			OnPropertyChanged(() => EnumAUProperties);
			UpdateDeviceParameterMissmatchType();
		}

		public void UpdateDeviceParameterMissmatchType()
		{
			if (StringAUProperties.Count + EnumAUProperties.Count > 0)
			{
				DeviceParameterMissmatchType maxDeviceParameterMissmatchType = DeviceParameterMissmatchType.Equal;
				foreach (var auProperty in StringAUProperties)
				{
					if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
						maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
				}
				foreach (var auProperty in EnumAUProperties)
				{
					if (auProperty.DeviceParameterMissmatchType > maxDeviceParameterMissmatchType)
						maxDeviceParameterMissmatchType = auProperty.DeviceParameterMissmatchType;
				}
				DeviceParameterMissmatchType = maxDeviceParameterMissmatchType;
			}
		}

		void On_AUParametersChanged()
		{
			Update();
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