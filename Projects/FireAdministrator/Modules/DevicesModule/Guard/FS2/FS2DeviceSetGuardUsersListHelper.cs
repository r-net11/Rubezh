using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.Guard
{
	public class FS2DeviceSetGuardUsersListHelper
	{
		static Device Device;
		static OperationResult OperationResult;
		static string Users;

		public static void Run(Device device, string users)
		{
			Device = device;
			Users = users;
			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Запись списка пользователей");
		}

		static void OnPropgress()
		{
			OperationResult = FiresecManager.FS2ClientContract.DeviceSetGuardUsers(Device.UID, Users);
		}

		static void OnCompleted()
		{
			if (OperationResult.HasError)
			{
				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
			}
			MessageBoxService.Show("Операция завершилась успешно");
		}
	}
}