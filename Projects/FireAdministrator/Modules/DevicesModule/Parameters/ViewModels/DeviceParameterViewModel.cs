using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using DevicesModule.DeviceProperties;
using Infrastructure.Common.TreeList;
using FiresecClient;

namespace DevicesModule.ViewModels
{
	public class DeviceParameterViewModel : TreeNodeViewModel<DeviceParameterViewModel>
	{
		public Device Device { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }
		public string PresentationZone { get; private set; }
		public bool HasAUParameters { get; private set; }
		public bool HasMissmatch { get; private set; }

		public DeviceParameterViewModel(Device device)
		{
			Device = device;
			PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);
			Update();
			device.AUParametersChanged += new Action(On_AUParametersChanged);
		}

		public void Update()
		{
			StringProperties = new List<StringPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			if (Device != null)
			{
				foreach (var driverProperty in Device.Driver.Properties)
				{
					if (driverProperty.IsAUParameter)
					{
						switch (driverProperty.DriverPropertyType)
						{
							case DriverPropertyTypeEnum.EnumType:
								EnumProperties.Add(new EnumPropertyViewModel(driverProperty, Device));
								break;

							case DriverPropertyTypeEnum.StringType:
							case DriverPropertyTypeEnum.IntType:
							case DriverPropertyTypeEnum.ByteType:
								StringProperties.Add(new StringPropertyViewModel(driverProperty, Device));
								break;

							case DriverPropertyTypeEnum.BoolType:
								BoolProperties.Add(new BoolPropertyViewModel(driverProperty, Device));
								break;
						}

						HasAUParameters = true;
					}
				}
			}
			OnPropertyChanged("StringProperties");
			OnPropertyChanged("BoolProperties");
			OnPropertyChanged("EnumProperties");
		}

		void On_AUParametersChanged()
		{
			Update();
			IsAuParametersReady = true;
		}

		bool _isAuParametersReady = true;
		public bool IsAuParametersReady
		{
			get { return _isAuParametersReady; }
			set
			{
				_isAuParametersReady = value;
				OnPropertyChanged("IsAuParametersReady");
			}
		}
	}
}