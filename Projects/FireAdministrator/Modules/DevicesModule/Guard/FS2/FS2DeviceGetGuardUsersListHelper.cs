using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.Guard
{
	public class FS2DeviceGetGuardUserListHelper
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
			OperationResult = FiresecManager.FS2ClientContract.DeviceGetGuardUsers(Device.UID);
		}

		static void OnCompleted()
		{
			if (OperationResult.HasError)
			{
				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			MessageBoxService.Show(OperationResult.Result);
			Result = OperationResult.Result;
		}
	}
}