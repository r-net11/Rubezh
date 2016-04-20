//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;
//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows;

//namespace DevicesModule.ViewModels
//{
//	public class FS2WriteAllDeviceConfigurationHelper
//	{
//		OperationResult<List<Guid>> OperationResult;
//		List<Guid> DeviceUIDs;

//		public void Run(List<Guid> deviceUIDs)
//		{
//			DeviceUIDs = deviceUIDs;
//			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, ". Запись конфигурации во все устройства");
//		}

//		void OnPropgress()
//		{
//			OperationResult = FiresecManager.FS2ClientContract.DeviceWriteAllConfiguration(DeviceUIDs, FiresecManager.CurrentUser.Name);
//		}

//		void OnCompleted()
//		{
//			if (OperationResult.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			if (OperationResult.Result.Count > 0)
//			{
//				var message = "Для устройств";
//				var devices = new List<Device>();
//				foreach (var deviceUID in OperationResult.Result)
//				{
//					var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
//					if (devices != null)
//					{
//						devices.Add(device);
//						message += "\n" + device.DottedPresentationNameAndAddress;
//					}
//				}
//				message += "\n" + "Операция записи закончилась неудачно. Повторить";
//				if (MessageBoxService.ShowQuestion(message) == MessageBoxResult.Yes)
//				{
//					var fs2WriteAllDeviceConfigurationHelper = new FS2WriteAllDeviceConfigurationHelper();
//					fs2WriteAllDeviceConfigurationHelper.Run(OperationResult.Result);
//				}
//			}
//			else
//			{
//				MessageBoxService.Show("Операция завершилась успешно");
//			}
//		}
//	}
//}