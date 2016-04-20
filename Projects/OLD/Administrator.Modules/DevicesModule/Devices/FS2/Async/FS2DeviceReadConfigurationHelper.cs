//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows.Windows;

//namespace DevicesModule.ViewModels
//{
//	public static class FS2DeviceReadConfigurationHelper
//	{
//		static Device Device;
//		static bool IsUsb;
//		static OperationResult<DeviceConfiguration> OperationResult;

//		public static void Run(Device device, bool isUsb)
//		{
//			Device = device;
//			IsUsb = isUsb;
//			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndName + ". Чтение конфигурации из устройства");
//		}

//		static void OnPropgress()
//		{
//			OperationResult = FiresecManager.FS2ClientContract.DeviceReadConfiguration(Device.UID, IsUsb, FiresecManager.CurrentUser.Name);
//		}

//		static void OnCompleted()
//		{
//			if (OperationResult.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			DialogService.ShowModalWindow(new DeviceConfigurationViewModel(Device.UID, OperationResult.Result));
//		}
//	}
//}