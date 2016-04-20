//using System.Collections.Generic;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure.Common.Windows.Windows;
//using Infrastructure.Common.Windows.Windows.ViewModels;

//namespace DevicesModule.ViewModels
//{
//	public class FS2CustomAdminFunctionsCommandViewModel : SaveCancelDialogViewModel
//	{
//		Device Device;
//		bool IsUsb;

//		public FS2CustomAdminFunctionsCommandViewModel(Device device, bool isUsb)
//		{
//			Device = device;
//			IsUsb = isUsb;
//			Title = "Выбор функции";

//			var operationResult = FiresecManager.FS2ClientContract.DeviceGetCustomFunctions(device.Driver.DriverType);
//			if (operationResult.HasError)
//			{
//				MessageBoxService.ShowError(operationResult.Error, "Ошибка при выполнении операции");
//				return;
//			}
//			Functions = operationResult.Result;
//		}

//		public List<DeviceCustomFunction> Functions { get; private set; }

//		DeviceCustomFunction _selectedFunction;
//		public DeviceCustomFunction SelectedFunction
//		{
//			get { return _selectedFunction; }
//			set
//			{
//				_selectedFunction = value;
//				OnPropertyChanged(()=>SelectedFunction);
//			}
//		}

//		protected override bool Save()
//		{
//			if (SelectedFunction != null)
//			{
//				FS2DeviceCustomFunctionExecuteHelper.Run(Device, IsUsb, SelectedFunction.Code);
//			}
//			return base.Save();
//		}
//	}
//}