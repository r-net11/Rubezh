using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.Guard
{
	public class DeviceGetGuardUserListHelper
	{
		static Device Device;
		static OperationResult<string> OperationResult;
		public static string Result { get; private set; }

		public static void Run(Device device)
		{
			Device = device;
			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Чтение списка пользователей");
		}

		static void OnPropgress()
		{
			OperationResult = FiresecManager.DeviceGetGuardUsersList(Device);
		}

		static void OnCompleted()
		{
			if (OperationResult.HasError)
			{
				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			Result = OperationResult.Result;
		}
	}
}