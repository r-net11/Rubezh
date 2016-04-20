using System.Collections.Generic;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKDevice Device { get; set; }

		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }

		public MPTDevicePropertiesViewModel(GKDevice device, bool showAll)
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
						continue;
					}
					switch (driverProperty.DriverPropertyType)
					{
						case GKDriverPropertyTypeEnum.StringType:
							var stringPropertyViewModel = new StringPropertyViewModel(driverProperty, Device);
							stringPropertyViewModel.PropertyChanged += propertyViewModel_PropertyChanged;
							StringProperties.Add(stringPropertyViewModel);
							break;
						case GKDriverPropertyTypeEnum.IntType:
							var shortPropertyViewModel = new ShortPropertyViewModel(driverProperty, Device);
							shortPropertyViewModel.PropertyChanged += propertyViewModel_PropertyChanged;
							ShortProperties.Add(shortPropertyViewModel);
							break;
						case GKDriverPropertyTypeEnum.BoolType:
							var boolPropertyViewModel = new BoolPropertyViewModel(driverProperty, Device);
							boolPropertyViewModel.PropertyChanged += propertyViewModel_PropertyChanged;
							BoolProperties.Add(boolPropertyViewModel);
							break;
						case GKDriverPropertyTypeEnum.EnumType:
							var enumPropertyViewModel = new EnumPropertyViewModel(driverProperty, Device);
							enumPropertyViewModel.PropertyChanged += propertyViewModel_PropertyChanged;
							EnumProperties.Add(enumPropertyViewModel);
							break;
					}
				}
			}

			OnPropertyChanged(() => StringProperties);
			OnPropertyChanged(() => ShortProperties);
			OnPropertyChanged(() => BoolProperties);
			OnPropertyChanged(() => EnumProperties);
		}

		void propertyViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			MPTViewModel.ChangeIsInMPT(Device, true);
		}
	}
}