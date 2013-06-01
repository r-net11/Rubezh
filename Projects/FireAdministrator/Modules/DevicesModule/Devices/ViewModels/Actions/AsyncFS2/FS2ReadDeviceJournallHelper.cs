using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
	public static class FS2ReadDeviceJournalHelper
	{
		static Device Device;
		static bool IsUsb;
		static OperationResult<string> OperationResult;

		public static void Run(Device device, bool isUsb)
		{
			Device = device;
			IsUsb = isUsb;

			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Чтение журнала");
		}

		static void OnPropgress()
		{
			OperationResult = FiresecManager.FS2ClientContract.DeviceReadEventLog(Device.UID, IsUsb);
		}

		static void OnCompleted()
		{
			if (OperationResult.HasError)
			{
				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			DialogService.ShowModalWindow(new DeviceJournalViewModel(OperationResult.Result));
		}
	}
}