using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
	public static class DeviceCustomFunctionExecuteHelper
	{
		static Device _device;
		static bool _isUsb;
		static string _functionCode;
		static OperationResult<string> _operationResult;

		public static void Run(Device device, bool isUsb, string functionCode)
		{
			_isUsb = isUsb;
			_device = device;
			_functionCode = functionCode;

			ServiceFactory.ProgressService.Run(OnPropgress, OnlCompleted, device.PresentationAddressAndName + ". Выполнение функции");
		}

		static void OnPropgress()
		{
			_operationResult = FiresecManager.DeviceCustomFunctionExecute(_device, _isUsb, _functionCode);
		}

		static void OnlCompleted()
		{
			if (_operationResult.HasError)
			{
				MessageBoxService.ShowErrorExtended(_operationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			var result = _operationResult.Result;
			result = result.Replace("[OK]", "");
			MessageBoxService.ShowExtended(result);
		}
	}
}