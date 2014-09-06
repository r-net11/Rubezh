using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure;
using FiresecClient;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class ControllerNetSettingsViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel DeviceViewModel { get; set; }
		bool HasChanged { get; set; }

		public ControllerNetSettingsViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Сетевые настройки контроллера";
			DeviceViewModel = deviceViewModel;

			WriteCommand = new RelayCommand(OnWrite);
			ReadCommand = new RelayCommand(OnRead);
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		string _mask;
		public string Mask
		{
			get { return _mask; }
			set
			{
				_mask = value;
				OnPropertyChanged(() => Mask);
			}
		}

		string _defaultGateway;
		public string DefaultGateway
		{
			get { return _defaultGateway; }
			set
			{
				_defaultGateway = value;
				OnPropertyChanged(() => DefaultGateway);
			}
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var controllerNetworkSettings = new SKDControllerNetworkSettings();
			controllerNetworkSettings.Address = Address;
			controllerNetworkSettings.Mask = Mask;
			controllerNetworkSettings.DefaultGateway = DefaultGateway;
			var result = FiresecManager.FiresecService.SetControllerNetworkSettings(DeviceViewModel.Device, controllerNetworkSettings);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				HasChanged = true;
			}
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			var result = FiresecManager.FiresecService.GetControllerNetworkSettings(DeviceViewModel.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				var controllerNetworkSettings = result.Result;
				Address = controllerNetworkSettings.Address;
				Mask = controllerNetworkSettings.Mask;
				DefaultGateway = controllerNetworkSettings.DefaultGateway;
				HasChanged = true;
			}
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				var addressProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "IPAddress");
				if (addressProperty == null)
				{
					MessageBoxService.ShowWarning("У контроллера отсутствует адрес");
					return false;
				}

				if (MessageBoxService.ShowQuestion2("Пароль в контроллере был изменен. Изменить пароль в конфигурации?"))
				{
					addressProperty.StringValue = Address;
					DeviceViewModel.UpdateProperties();
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
			return base.Save();
		}
	}
}