using System;
using System.Collections.Generic;
using System.Text;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
	public static class FS2WriteAllDeviceConfigurationHelper
	{
		static OperationResult OperationResult;

		public static void Run()
		{
			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, ". Запись конфигурации во все устройства");
		}

		static void OnPropgress()
		{
			OperationResult = FiresecManager.FS2ClientContract.DeviceWriteAllConfiguration();
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