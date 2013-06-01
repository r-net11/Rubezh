using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class PropertiesViewModel : BaseViewModel
	{
		public XDevice XDevice { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }

		public PropertiesViewModel(XDevice device)
		{
			XDevice = device;
			StringProperties = new List<StringPropertyViewModel>();
			ShortProperties = new List<ShortPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();

			if (device == null)
				return;
			foreach (var driverProperty in device.Driver.Properties)
			{
				switch (driverProperty.DriverPropertyType)
				{
					case XDriverPropertyTypeEnum.EnumType:
						EnumProperties.Add(new EnumPropertyViewModel(driverProperty, device));
						break;

					case XDriverPropertyTypeEnum.StringType:
						StringProperties.Add(new StringPropertyViewModel(driverProperty, device));
						break;

					case XDriverPropertyTypeEnum.IntType:
						ShortProperties.Add(new ShortPropertyViewModel(driverProperty, device));
						break;

					case XDriverPropertyTypeEnum.BoolType:
						BoolProperties.Add(new BoolPropertyViewModel(driverProperty, device));
						break;
				}
			}
		}
	}
}