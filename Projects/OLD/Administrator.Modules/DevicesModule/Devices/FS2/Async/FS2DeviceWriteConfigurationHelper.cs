//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows.Windows;

//namespace DevicesModule.ViewModels
//{
//	public static class FS2DeviceWriteConfigurationHelper
//	{
//		static Device Device;
//		static bool IsUsb;
//		static OperationResult<bool> OperationResult;

//		public static void Run(Device device, bool isUsb)
//		{
//			Device = device;
//			IsUsb = isUsb;
//			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Запись конфигурации в устройство");
//		}

//		static void OnPropgress()
//		{
//			OperationResult = FiresecManager.FS2ClientContract.DeviceWriteConfiguration(Device.UID, IsUsb, FiresecManager.CurrentUser.Name);
//		}

//		static void OnCompleted()
//		{
//			if (OperationResult.HasError || !OperationResult.Result)
//			{
//				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			MessageBoxService.Show("Операция завершилась успешно");
//		}
//	}
//}