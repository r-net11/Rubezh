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
		public List<StringAUPropertyViewModel> StringAUProperties { get; set; }
		public List<EnumAUPropertyViewModel> EnumAUProperties { get; set; }
		public string PresentationZone { get; private set; }
		public bool HasAUParameters { get; private set; }

		public DeviceParameterViewModel(Device device)
		{
			Device = device;
			PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);
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
							case DriverPropertyTypeEnum.EnumType:
								EnumAUProperties.Add(new EnumAUPropertyViewModel(driverProperty, Device));
								break;

							case DriverPropertyTypeEnum.StringType:
							case DriverPropertyTypeEnum.IntType:
							case DriverPropertyTypeEnum.ByteType:
								StringAUProperties.Add(new StringAUPropertyViewModel(driverProperty, Device));
								break;
						}

						HasAUParameters = true;
					}
				}
			}

			OnPropertyChanged("StringAUProperties");
			OnPropertyChanged("EnumAUProperties");
			UpdateIsMissmatch();
		}

		public void UpdateIsMissmatch()
		{
			if (EnumAUProperties.Any(x => x.IsMissmatch) || StringAUProperties.Any(x => x.IsMissmatch))
				IsMissmatch = true;
		}

		void On_AUParametersChanged()
		{
			Update();
			IsAuParametersReady = true;
		}

		bool _isMissmatch;
		public bool IsMissmatch
		{
			get { return _isMissmatch; }
			set
			{
				_isMissmatch = value;
				OnPropertyChanged("IsMissmatch");
			}
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