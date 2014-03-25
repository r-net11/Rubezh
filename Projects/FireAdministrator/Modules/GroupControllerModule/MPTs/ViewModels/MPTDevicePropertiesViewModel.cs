using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		public XDevice Device { get; set; }

		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }

		public MPTDevicePropertiesViewModel(XDevice device, bool showAll)
		{
			Title = "Настройка свойств устройства";
			Device = device;
			Update(showAll);
		}

		public void Update(bool showAll)
		{
			StringProperties = new List<StringPropertyViewModel>();
			ShortProperties = new List<ShortPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			if (Device != null)
			{
				foreach (var driverProperty in Device.Driver.Properties)
				{
					if (!showAll)
					{
						if (Device.DriverType == XDriverType.RSR2_AM_1)
							continue;
						if (driverProperty.Name != "Задержка на включение, с" && driverProperty.Name != "Время удержания, с" && driverProperty.Name != "Контроль")
							continue;

					}
					switch (driverProperty.DriverPropertyType)
					{
						case XDriverPropertyTypeEnum.StringType:
							StringProperties.Add(new StringPropertyViewModel(driverProperty, Device));
							break;
						case XDriverPropertyTypeEnum.IntType:
							ShortProperties.Add(new ShortPropertyViewModel(driverProperty, Device));
							break;
						case XDriverPropertyTypeEnum.BoolType:
							BoolProperties.Add(new BoolPropertyViewModel(driverProperty, Device));
							break;
						case XDriverPropertyTypeEnum.EnumType:
							EnumProperties.Add(new EnumPropertyViewModel(driverProperty, Device));
							break;
					}
				}
			}

			OnPropertyChanged("StringProperties");
			OnPropertyChanged("ShortProperties");
			OnPropertyChanged("BoolProperties");
			OnPropertyChanged("EnumProperties");
		}
	}
}