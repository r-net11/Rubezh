using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using System;

namespace DevicesModule.ViewModels
{
	public static class FS2SynchronizeDeviceHelper
	{
		static Device Device;
		static bool IsUsb;
		static OperationResult<bool> OperationResult;

		public static void Run(Device device, bool isUsb)
		{
			Device = device;
			IsUsb = isUsb;
			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndName + ". Установка времени");
		}

		static void OnPropgress()
		{
			//FiresecManager.FS2ClientContract.CancelPoll(new Guid());
			//FiresecManager.FS2ClientContract.CancelProgress();
			//OperationResult = new OperationResult<bool>();
			OperationResult = FiresecManager.FS2ClientContract.DeviceDatetimeSync(Device.UID, IsUsb);
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