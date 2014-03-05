using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
	public static class SetPasswordHelper
	{
		static Device _device;
		static bool _isUsb;
		static DevicePasswordType _devicePasswordType;
		static string _password;
		static OperationResult<bool> _operationResult;

		public static void Run(Device device, bool isUsb, DevicePasswordType devicePasswordType, string password)
		{
			_device = device;
			_isUsb = isUsb;
			_devicePasswordType = devicePasswordType;
			_password = password;

			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndName + ". Установка пароля");
		}

		static void OnPropgress()
		{
			_operationResult = FiresecManager.SetPassword(_device, _isUsb, _devicePasswordType, _password);
		}

		static void OnCompleted()
		{
			if (_operationResult.HasError)
			{
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			MessageBoxService.Show("Операция завершилась успешно");
		}
	}
}