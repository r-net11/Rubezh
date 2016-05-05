using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure;
using FiresecClient;
using StrazhAPI.SKD;

namespace StrazhModule.ViewModels
{
	public class ControllerNetSettingsViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel DeviceViewModel { get; set; }
		bool HasChanged { get; set; }

		private bool NeedSaveChangesToController { get; set; }

		public ControllerNetSettingsViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Сетевые настройки контроллера";
			DeviceViewModel = deviceViewModel;

			WriteCommand = new RelayCommand(OnWrite);
			ReadCommand = new RelayCommand(OnRead);

			var addressProperty = deviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Address");
			if (addressProperty != null)
			{
				Address = addressProperty.StringValue;
			}
			var maskProperty = deviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Mask");
			if (maskProperty != null)
			{
				Mask = maskProperty.StringValue;
			}
			var gatewayProperty = deviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Gateway");
			if (gatewayProperty != null)
			{
				DefaultGateway = gatewayProperty.StringValue;
			}

			HasChanged = false;
			NeedSaveChangesToController = false;
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				if (_address == value)
					return;
				_address = value;
				OnPropertyChanged(() => Address);
				HasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		string _mask;
		public string Mask
		{
			get { return _mask; }
			set
			{
				if (_mask == value)
					return;
				_mask = value;
				OnPropertyChanged(() => Mask);
				HasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		string _defaultGateway;
		public string DefaultGateway
		{
			get { return _defaultGateway; }
			set
			{
				if (_defaultGateway == value)
					return;
				_defaultGateway = value;
				OnPropertyChanged(() => DefaultGateway);
				HasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var sb = new StringBuilder();
			if (!SKDManager.ValidateIPAddress(Address))
				sb.AppendLine("Не верно задан адрес");
			if (!SKDManager.ValidateIPAddress(Mask))
				sb.AppendLine("Не верно задана маска подсети");
			if (!SKDManager.ValidateIPAddress(DefaultGateway))
				sb.AppendLine("Не верно задан шлюз по умолчанию");
			if (sb.Length > 0)
			{
				MessageBoxService.ShowWarning(sb.ToString());
				return;
			}

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

			RebootController();
			//HasChanged = true;
			NeedSaveChangesToController = false;
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
			var controllerNetworkSettings = result.Result;
			Address = controllerNetworkSettings.Address;
			Mask = controllerNetworkSettings.Mask;
			DefaultGateway = controllerNetworkSettings.DefaultGateway;
			HasChanged = true;
			NeedSaveChangesToController = false;
		}

		void RebootController()
		{
			var result = FiresecManager.FiresecService.SKDRebootController(DeviceViewModel.Device);
			if (result.Result)
			{
				MessageBoxService.Show("Выполняется перезагрузка контроллера. Контроллер будет доступен через несколько секунд");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции", result.Error);
			}
		}

		protected override bool Save()
		{
			// Нет изменений, поэтому просто выходим
			if (!HasChanged)
				return base.Save();

			// Есть изменения, но они не записаны на контроллер и пользователь отказался от сохранения изменений в конфигурации, поэтому не закрываем окно
			if (NeedSaveChangesToController && !MessageBoxService.ShowQuestion("Сетевые настройки не были записаны на контроллер. Изменить сетевые настройки в конфигурации?"))
				return false;

			// Сохраняем изменения в конфигурации
			var addressProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Address");
			if (addressProperty != null)
				addressProperty.StringValue = Address;

			var maskProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Mask");
			if (maskProperty != null)
				maskProperty.StringValue = Mask;

			var gatewayProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Gateway");
			if (gatewayProperty != null)
				gatewayProperty.StringValue = DefaultGateway;

			DeviceViewModel.UpdateProperties();
			ServiceFactory.SaveService.SKDChanged = true;
			return base.Save();
		}
	}
}