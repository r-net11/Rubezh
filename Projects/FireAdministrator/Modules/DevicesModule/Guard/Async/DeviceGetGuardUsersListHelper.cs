using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.Guard
{
	public class DeviceGetGuardUserListHelper
	{
		static Device _device;
		static OperationResult<string> _operationResult;

		public static void Run(Device device)
		{
			_device = device;
			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressAndDriver + ". Чтение списка пользователей");
		}

		static void OnPropgress()
		{
			_operationResult = FiresecManager.DeviceGetGuardUsersList(_device);
		}

		static void OnCompleted()
		{
			if (_operationResult.HasError)
			{
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			MessageBoxService.Show(_operationResult.Result);
		}
	}
}