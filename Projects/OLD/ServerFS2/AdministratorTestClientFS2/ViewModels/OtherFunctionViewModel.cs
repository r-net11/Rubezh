using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ServerFS2;
using ServerFS2.Operations;

namespace AdministratorTestClientFS2.ViewModels
{
	public class OtherFunctionViewModel : DialogViewModel
	{
		public OtherFunctionViewModel(Device device)
		{
			Title = "Выбор функции";
			selectedDeivce = device;
			OtherFunctions = DeviceCustomFunctionListHelper.GetDeviceCustomFunctionList(device.Driver.DriverType);
			OkCommand = new RelayCommand(OnOk);
			CancelCommand = new RelayCommand(OnClose);
		}
		private Device selectedDeivce;

		private List<DeviceCustomFunction> otherFunctions;
		public List<DeviceCustomFunction> OtherFunctions
		{
			get { return otherFunctions; }
			set
			{
				otherFunctions = value;
				OnPropertyChanged("OtherFunctions");
			}
		}

		private DeviceCustomFunction selectedFunction;
		public DeviceCustomFunction SelectedFunction
		{
			get { return selectedFunction; }
			set
			{
				selectedFunction = value;
				OnPropertyChanged("SelectedFunction");
			}
		}

		public RelayCommand OkCommand { get; private set; }
		void OnOk()
		{
			TouchMemoryOperationHelper.Operation(selectedDeivce, SelectedFunction.No);
			Close();
			if (SelectedFunction.No == 0)
				MessageBoxService.ShowWarning("Приложенный в течении 30 секунд ключ будет назначен мастер-ключом");
		}

		public RelayCommand CancelCommand { get; private set; }
		void OnClose()
		{
			Close();
		}
	}
}
