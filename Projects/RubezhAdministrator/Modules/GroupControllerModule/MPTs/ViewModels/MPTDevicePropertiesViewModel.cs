using System.Collections.Generic;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using GKProcessor;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure;
using RubezhAPI;
using System.Linq;
using System;
using System.Threading;

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
			ReadPropertiesCommand = new RelayCommand(OnReadProperties, CanReadWrite);
			WritePropertiesCommand = new RelayCommand(OnWriteProperties, CanReadWrite);

			Title = "Настройка свойств устройства";
			Device = device;
			Update(showAll);
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			if (!DeviceViewModel.CompareLocalWithRemoteHashes(Device))
				return;

			var result = ClientManager.RubezhService.GKGetSingleParameter(Device);
			if (!result.HasError)
			{
				foreach (var property in result.Result)
				{
					var deviceProperty = Device.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (deviceProperty == null)
					{
						deviceProperty = new GKProperty()
						{
							Name = property.Name,
							DriverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name)
						};
						Device.DeviceProperties.Add(deviceProperty);
					}
					deviceProperty.Value = property.Value;
					deviceProperty.StringValue = property.StringValue;
				}
				Update(true);
			}
			else
			{
				MessageBoxService.ShowError(result.Error);
			}


		}
		public RelayCommand WritePropertiesCommand { get; private set; }
		void OnWriteProperties()
		{
			DescriptorsManager.Create();
			if (!DeviceViewModel.CompareLocalWithRemoteHashes(Device))
				return;
			if (DeviceViewModel.Validate(new List<GKDevice>() { Device }))
			{
				DescriptorsManager.Create();

				var baseDescriptor = ParametersHelper.GetBaseDescriptor(Device);
				if (baseDescriptor != null)
				{
					var result = ClientManager.RubezhService.GKSetSingleParameter(Device, baseDescriptor.Parameters, Device.Properties);
					if (result.HasError)
					{
						MessageBoxService.ShowError(result.Error);
					}
				}
				else
				{
					MessageBoxService.ShowError("Ошибка. Отсутствуют параметры");
				}
			}
		}
		bool CanReadWrite()
		{
			return Device.Driver.Properties.Count(x => x.IsAUParameter) > 0;
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