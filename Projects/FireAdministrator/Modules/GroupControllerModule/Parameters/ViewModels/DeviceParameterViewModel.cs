using System;
using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.DeviceProperties;
using Infrastructure.Common.Windows.TreeList;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DeviceParameterViewModel : TreeNodeViewModel<DeviceParameterViewModel>
	{
		public GKDevice Device { get; private set; }
		public List<StringAUPropertyViewModel> StringAUProperties { get; set; }
		public List<EnumAUPropertyViewModel> EnumAUProperties { get; set; }
		public string PresentationZone { get; private set; }
		public bool HasAUParameters { get; private set; }

		public DeviceParameterViewModel(GKDevice device)
		{
			Device = device;
			PresentationZone = GKManager.GetPresentationZoneOrLogic(Device);
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
					if (driverProperty.CanNotEdit)
						continue;

					if (driverProperty.IsAUParameter)
					{
						switch (driverProperty.DriverPropertyType)
						{
							case GKDriverPropertyTypeEnum.EnumType:
								EnumAUProperties.Add(new EnumAUPropertyViewModel(driverProperty, Device));
								break;

							case GKDriverPropertyTypeEnum.StringType:
							case GKDriverPropertyTypeEnum.IntType:
							case GKDriverPropertyTypeEnum.BoolType:
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
				OnPropertyChanged(() => DeviceParameterMissmatchType);
			}
		}
	}
}