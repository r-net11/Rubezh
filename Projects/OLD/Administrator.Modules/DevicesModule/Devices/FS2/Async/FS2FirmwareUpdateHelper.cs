//using System.IO;
//using System.Windows;
//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows.Windows;
//using Microsoft.Win32;

//namespace DevicesModule.ViewModels
//{
//	public static class FS2FirmwareUpdateHelper
//	{
//		static Device Device;
//		static bool IsUsb;
//		static string FileName;
//		static OperationResult<string> OperationResult_1;
//		static OperationResult OperationResult_2;

//		public static void Run(Device device, bool isUsb)
//		{
//			Device = device;
//			IsUsb = isUsb;

//			var openFileDialog = new OpenFileDialog()
//			{
//				Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.FSCF)|*.FSCF|All files (*.*)|*.*"
//			};
//			if (openFileDialog.ShowDialog() == true)
//			{
//				FileName = openFileDialog.FileName;
//				ServiceFactory.FS2ProgressService.Run(OnVarifyPropgress, OnVerifyCompleted, Device.PresentationAddressAndName + ". Обновление прошивки");
//			}
//		}

//		static void OnVarifyPropgress()
//		{
//			OperationResult_1 = FiresecManager.FS2ClientContract.DeviceVerifyFirmwareVersion(Device.UID, IsUsb, new FileInfo(FileName).FullName);
//		}

//		static void OnVerifyCompleted()
//		{
//			if (OperationResult_1.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult_1.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			else
//			{
//				if (MessageBoxService.ShowQuestion(OperationResult_1.Result) == MessageBoxResult.Yes)
//				{
//					ServiceFactory.FS2ProgressService.Run(OnUpdatePropgress, OnUpdateCompleted, Device.PresentationAddressAndName + ". Обновление прошивки");
//				}
//			}
//		}

//		static void OnUpdatePropgress()
//		{
//			OperationResult_2 = FiresecManager.FS2ClientContract.DeviceUpdateFirmware(Device.UID, IsUsb, new FileInfo(FileName).FullName, FiresecManager.CurrentUser.Name);
//		}

//		static void OnUpdateCompleted()
//		{
//			if (OperationResult_2.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult_2.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			MessageBoxService.Show("Операция завершилась успешно");
//		}
//	}
//}