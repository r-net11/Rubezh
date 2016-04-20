//using System.Collections.Generic;
//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows;

//namespace DevicesModule.ViewModels
//{
//	public static class FS2DeviceGetSerialListHelper
//	{
//		static Device Device;
//		static OperationResult<List<string>> OperationResult;

//		public static void Run(Device device)
//		{
//			Device = device;
//			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnlCompleted, Device.PresentationAddressAndName + ". Получение списка устройств");
//		}

//		static void OnPropgress()
//		{
//			OperationResult = FiresecManager.FS2ClientContract.DeviceGetSerialList(Device.UID, FiresecManager.CurrentUser.Name);
//		}

//		static void OnlCompleted()
//		{
//			if (OperationResult.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			DialogService.ShowModalWindow(new BindMsViewModel(Device, OperationResult.Result));
//		}
//	}
//}