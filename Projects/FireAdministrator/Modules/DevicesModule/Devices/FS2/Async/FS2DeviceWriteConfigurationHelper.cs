using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
	public static class FS2DeviceWriteConfigurationHelper
	{
		static Device Device;
		static bool IsUsb;
		static OperationResult OperationResult;

		public static void Run(Device device, bool isUsb)
		{
			Device = device;
			IsUsb = isUsb;
			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Запись конфигурации в устройство");
		}

		static void OnPropgress()
		{
			OperationResult = FiresecManager.FS2ClientContract.DeviceWriteConfig(Device.UID, IsUsb);
		}

		static void OnCompleted()
		{
			if (OperationResult.HasError)
			{
				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			MessageBoxService.Show("Операция завершилась успешно");
		}
	}
}