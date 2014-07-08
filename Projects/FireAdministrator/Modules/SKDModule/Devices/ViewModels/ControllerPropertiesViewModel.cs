using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.ComponentModel;
using Infrastructure.Events;

namespace SKDModule.ViewModels
{
	public class ControllerPropertiesViewModel : DialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public SKDDeviceInfo DeviceInfo { get; private set; }

		public ControllerPropertiesViewModel(SKDDevice device)
		{
			Title = "Конфигурация контроллера";
			Device = device;
			GetPasswordCommand = new RelayCommand(OnGetPassword);
			SetPasswordCommand = new RelayCommand(OnSetPassword);
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime);
			ResetCommand = new RelayCommand(OnReset);
			RebootCommand = new RelayCommand(OnReboot);

			var deviceInfoResult = FiresecManager.FiresecService.SKDGetDeviceInfo(Device);
			if (deviceInfoResult.HasError)
			{
				Close(true);
				MessageBoxService.ShowWarning(deviceInfoResult.Error, "Нет связи с устройством");
				return;
			}
			else
			{
				DeviceInfo = deviceInfoResult.Result;
			}

			var passwordResult = FiresecManager.FiresecService.SKDGetPassword(Device.UID);
			if (passwordResult.HasError)
			{
				Close(true);
				MessageBoxService.ShowWarning(deviceInfoResult.Error, "Нет связи с устройством");
				return;
			}
			else
			{
				Password = passwordResult.Result;
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		public RelayCommand GetPasswordCommand { get; private set; }
		void OnGetPassword()
		{
			var result = FiresecManager.FiresecService.SKDGetPassword(Device.UID);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				Password = result.Result;
			}
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		void OnSetPassword()
		{
			var result = FiresecManager.FiresecService.SKDSetPassword(Device.UID, Password);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = FiresecManager.FiresecService.SKDSyncronyseTime(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция синхронизации времени завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка во время операции синхронизации времени");
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			var result = FiresecManager.FiresecService.SKDResetController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка во время операции");
			}
		}

		public RelayCommand RebootCommand { get; private set; }
		void OnReboot()
		{
			var result = FiresecManager.FiresecService.SKDRebootController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка во время операции");
			}
		}
	}
}