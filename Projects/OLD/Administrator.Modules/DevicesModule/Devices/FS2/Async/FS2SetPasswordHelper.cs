//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows.Windows;

//namespace DevicesModule.ViewModels
//{
//	public static class FS2SetPasswordHelper
//	{
//		static Device Device;
//		static bool IsUsb;
//		static DevicePasswordType DevicePasswordType;
//		static string Password;
//		static OperationResult OperationResult;

//		public static void Run(Device device, bool isUsb, DevicePasswordType devicePasswordType, string password)
//		{
//			Device = device;
//			IsUsb = isUsb;
//			DevicePasswordType = devicePasswordType;
//			Password = password;

//			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndName + ". Установка пароля");
//		}

//		static void OnPropgress()
//		{
//			OperationResult = FiresecManager.FS2ClientContract.DeviceSetPassword(Device.UID, IsUsb, DevicePasswordType, Password, FiresecManager.CurrentUser.Name);
//		}

//		static void OnCompleted()
//		{
//			if (OperationResult.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			MessageBoxService.Show("Операция завершилась успешно");
//		}
//	}
//}