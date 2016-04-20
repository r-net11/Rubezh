using System.Collections.Generic;
using System.Diagnostics;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class CustomAdminFunctionsCommandViewModel : SaveCancelDialogViewModel
	{
		Device _device;
		bool _isUsb;

		public CustomAdminFunctionsCommandViewModel(Device device, bool isUsb)
		{
			_device = device;
			_isUsb = isUsb;
			Title = "Выбор функции";

			var operationResult = FiresecManager.FiresecDriver.DeviceCustomFunctionList(device.Driver.UID);
			if (operationResult.HasError)
			{
				MessageBoxService.ShowError(operationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			Functions = operationResult.Result;
			foreach (var function in Functions)
			{
				Trace.WriteLine(device.Driver.ShortName + " " + function.Code + " " + function.Description + " " + function.Name);
			}
		}

		public List<DeviceCustomFunction> Functions { get; private set; }

		DeviceCustomFunction _selectedFunction;
		public DeviceCustomFunction SelectedFunction
		{
			get { return _selectedFunction; }
			set
			{
				_selectedFunction = value;
				OnPropertyChanged(() => SelectedFunction);
			}
		}

		protected override bool Save()
		{
			if (SelectedFunction != null)
			{
				DeviceCustomFunctionExecuteHelper.Run(_device, _isUsb, SelectedFunction.Code);
			}
			return base.Save();
		}
	}
}