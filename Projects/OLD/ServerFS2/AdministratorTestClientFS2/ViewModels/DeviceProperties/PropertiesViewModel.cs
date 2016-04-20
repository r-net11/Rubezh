using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AdministratorTestClientFS2.ViewModels
{
	public class PropertiesViewModel : BaseViewModel
	{
		public static DevicesViewModel Context { get; private set; }

		public PropertiesViewModel(DevicesViewModel deviceViewModel)
		{
			Context = deviceViewModel;
		}

		public PropertiesViewModel(Device device)
		{
			Device = device;
			StringProperties = new List<StringPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			if (device.Driver == null)
				return;
			foreach (var driverProperty in device.Driver.Properties)
			{
				if (device.Driver.DriverType == DriverType.Exit)
					continue;

				if (device.Driver.DriverType == DriverType.PumpStation && driverProperty.Name == "AllowControl")
					continue;

				if (driverProperty.IsHidden || driverProperty.IsControl)
					continue;

				switch (driverProperty.DriverPropertyType)
				{
					case DriverPropertyTypeEnum.EnumType:
						EnumProperties.Add(new EnumPropertyViewModel(driverProperty, device));
						break;

					case DriverPropertyTypeEnum.StringType:
					case DriverPropertyTypeEnum.IntType:
					case DriverPropertyTypeEnum.ByteType:
						StringProperties.Add(new StringPropertyViewModel(driverProperty, device));
						break;

					case DriverPropertyTypeEnum.BoolType:
						BoolProperties.Add(new BoolPropertyViewModel(driverProperty, device));
						break;
				}
			}
		}

		public Device Device { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }
	}
}