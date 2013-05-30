using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
	public static class FS2DeviceReadConfigurationHelper
	{
		static Device _device;
		static bool _isUsb;
		static OperationResult<DeviceConfiguration> _operationResult;

		public static void Run(Device device, bool isUsb)
		{
			_device = device;
			_isUsb = isUsb;
			var fs2ProgressService = new FS2ProgressService();
			fs2ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndName + ". Чтение конфигурации из устройства");
		}

		static void OnPropgress()
		{
			_operationResult = FiresecManager.FS2ClientContract.DeviceReadConfig(_device.UID, _isUsb);
		}

		static void OnCompleted()
		{
			if (_operationResult.HasError)
			{
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			DialogService.ShowModalWindow(new DeviceConfigurationViewModel(_device.UID, _operationResult.Result));
		}
	}
}