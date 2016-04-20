using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;

namespace DevicesModule.Guard
{
	public class DeviceSetGuardUsersListHelper
	{
		static Device Device;
		static OperationResult<bool> OperationResult;
		static string Users;

		public static void Run(Device device, string users)
		{
			Device = device;
			Users = users;
			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Запись списка пользователей");
		}

		static void OnPropgress()
		{
			OperationResult = FiresecManager.DeviceSetGuardUsersList(Device, Users);
		}

		static void OnCompleted()
		{
			if (OperationResult.HasError)
			{
				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
			}
			else
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
		}
	}
}