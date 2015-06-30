//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows;

//namespace DevicesModule.ViewModels
//{
//	public static class FS2DeviceGetInformationHelper
//	{
//		static Device Device;
//		static bool IsUsb;
//		static OperationResult<string> OperationResult;

//		public static void Run(Device device, bool isUsb)
//		{
//			Device = device;
//			IsUsb = isUsb;
//			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Чтение информации об устройстве");
//		}

//		static void OnPropgress()
//		{
//			OperationResult = FiresecManager.FS2ClientContract.DeviceGetInformation(Device.UID, IsUsb, FiresecManager.CurrentUser.Name);
//		}

//		static void OnCompleted()
//		{
//			if (OperationResult.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			DialogService.ShowModalWindow(new DeviceDescriptionViewModel(Device, OperationResult.Result));
//		}
//	}
//}