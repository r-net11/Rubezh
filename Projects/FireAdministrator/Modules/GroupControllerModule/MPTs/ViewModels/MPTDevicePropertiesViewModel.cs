using System.Collections.Generic;
using FiresecAPI.GK;
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
							var stringPropertyViewModel = new StringPropertyViewModel(driverProperty, Device);
							stringPropertyViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(propertyViewModel_PropertyChanged);
							StringProperties.Add(stringPropertyViewModel);
							break;
						case XDriverPropertyTypeEnum.IntType:
							var shortPropertyViewModel = new ShortPropertyViewModel(driverProperty, Device);
							shortPropertyViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(propertyViewModel_PropertyChanged);
							ShortProperties.Add(shortPropertyViewModel);
							break;
						case XDriverPropertyTypeEnum.BoolType:
							var boolPropertyViewModel = new BoolPropertyViewModel(driverProperty, Device);
							boolPropertyViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(propertyViewModel_PropertyChanged);
							BoolProperties.Add(boolPropertyViewModel);
							break;
						case XDriverPropertyTypeEnum.EnumType:
							var enumPropertyViewModel = new EnumPropertyViewModel(driverProperty, Device);
							enumPropertyViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(propertyViewModel_PropertyChanged);
							EnumProperties.Add(enumPropertyViewModel);
							break;
					}
				}
			}

			OnPropertyChanged("StringProperties");
			OnPropertyChanged("ShortProperties");
			OnPropertyChanged("BoolProperties");
			OnPropertyChanged("EnumProperties");
		}

		void propertyViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			MPTViewModel.ChangeIsInMPT(Device, true);
		}
	}
}