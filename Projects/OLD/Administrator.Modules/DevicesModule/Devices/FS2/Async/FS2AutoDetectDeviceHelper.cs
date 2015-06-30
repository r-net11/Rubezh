//using System.Linq;
//using Common;
//using FiresecAPI;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common.Windows;

//namespace DevicesModule.ViewModels
//{
//	public static class FS2AutoDetectDeviceHelper
//	{
//		static Device Device;
//		static OperationResult<DeviceConfiguration> OperationResult;
//		static bool FastSearch;

//		public static void Run(Device device)
//		{
//			Device = device;
//			RunAutodetection(true);
//		}

//		static void RunAutodetection(bool fastSearch)
//		{
//			FastSearch = fastSearch;
//			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Автопоиск устройств");
//		}

//		static void OnPropgress()
//		{
//			OperationResult = FiresecManager.FS2ClientContract.DeviceAutoDetectChildren(Device.UID, FastSearch, FiresecManager.CurrentUser.Name);
//		}

//		static void OnCompleted()
//		{
//			if (OperationResult.HasError)
//			{
//				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
//				Logger.Error("AutoDetectDeviceHelper.OnCompleted " + OperationResult.Error);
//				return;
//			}

//			var deviceConfiguration = OperationResult.Result;
//			deviceConfiguration.Update();
//			foreach (var device in deviceConfiguration.Devices)
//			{
//				var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID.ToString().ToUpper() == device.DriverUID.ToString().ToUpper());
//				if (driver == null)
//				{
//					;
//				}
//				device.Driver = driver;

//			}


//			deviceConfiguration.UpdateIdPath();

//			var autodetectionViewModel = new AutoSearchViewModel(deviceConfiguration);
//			if (DialogService.ShowModalWindow(autodetectionViewModel))
//				RunAutodetection(false);
//		}
//	}
//}