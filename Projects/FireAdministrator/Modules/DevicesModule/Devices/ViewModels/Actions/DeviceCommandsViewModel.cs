using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel _devicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			AutoDetectCommand = new RelayCommand(OnAutoDetect, CanAutoDetect);
			ReadDeviceCommand = new RelayCommand<bool>(OnReadDevice, CanReadDevice);
			WriteDeviceCommand = new RelayCommand<bool>(OnWriteDevice, CanWriteDevice);
			WriteAllDeviceCommand = new RelayCommand(OnWriteAllDevice);
			SynchronizeDeviceCommand = new RelayCommand<bool>(OnSynchronizeDevice, CanSynchronizeDevice);
			UpdateSoftCommand = new RelayCommand<bool>(OnUpdateSoft, CanUpdateSoft);
			GetDescriptionCommand = new RelayCommand<bool>(OnGetDescription, CanGetDescription);
			GetDeveceJournalCommand = new RelayCommand<bool>(OnGetDeveceJournal, CanGetDeveceJournal);
			SetPasswordCommand = new RelayCommand<bool>(OnSetPassword, CanSetPassword);
			BindMsCommand = new RelayCommand(OnBindMs, CanBindMs);
			ExecuteCustomAdminFunctionsCommand = new RelayCommand<bool>(OnExecuteCustomAdminFunctions, CanExecuteCustomAdminFunctions);
			GetConfigurationParametersCommand = new RelayCommand(OnGetConfigurationParameters, CanGetConfigurationParameters);
			SetConfigurationParametersCommand = new RelayCommand(OnSetConfigurationParameters, CanSetConfigurationParameters);

			_devicesViewModel = devicesViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand AutoDetectCommand { get; private set; }
		void OnAutoDetect()
		{
			AutoDetectDeviceHelper.Run(SelectedDevice);
		}

		bool CanAutoDetect()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanAutoDetect));
		}

		public RelayCommand<bool> ReadDeviceCommand { get; private set; }
		void OnReadDevice(bool isUsb)
		{
			DeviceReadConfigurationHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanReadDevice(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReadDatabase));
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.CannotSave("FS") || validationResult.CannotWrite("FS"))
			{
				MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
				return false;
			}
			if (validationResult.HasErrors("FS"))
			{
				if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить") != MessageBoxResult.Yes)
					return false;
			}
			return true;
		}

		public RelayCommand<bool> WriteDeviceCommand { get; private set; }
		void OnWriteDevice(bool isUsb)
		{
			if (ValidateConfiguration())
				DeviceWriteConfigurationHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanWriteDevice(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanWriteDatabase));
		}

		public RelayCommand WriteAllDeviceCommand { get; private set; }
		void OnWriteAllDevice()
		{
			if (ValidateConfiguration())
			{
				WriteAllDeviceConfigurationHelper.Run();
			}
		}

		public RelayCommand<bool> SynchronizeDeviceCommand { get; private set; }
		void OnSynchronizeDevice(bool isUsb)
		{
			SynchronizeDeviceHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanSynchronizeDevice(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSynchonize));
		}

		bool CanRebootDevice()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReboot));
		}

		public RelayCommand<bool> UpdateSoftCommand { get; private set; }
		void OnUpdateSoft(bool isUsb)
		{
			FirmwareUpdateHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanUpdateSoft(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanUpdateSoft));
		}

		public RelayCommand<bool> GetDescriptionCommand { get; private set; }
		void OnGetDescription(bool isUsb)
		{
			DeviceGetInformationHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanGetDescription(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanGetDescription));
		}

		public RelayCommand<bool> GetDeveceJournalCommand { get; private set; }
		void OnGetDeveceJournal(bool isUsb)
		{
			ReadDeviceJournalHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanGetDeveceJournal(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReadJournal));
		}

		public RelayCommand<bool> SetPasswordCommand { get; private set; }
		void OnSetPassword(bool isUsb)
		{
			DialogService.ShowModalWindow(new SetPasswordViewModel(SelectedDevice.Device, isUsb));
		}

		bool CanSetPassword(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSetPassword));
		}

		public RelayCommand BindMsCommand { get; private set; }
		void OnBindMs()
		{
			DeviceGetSerialListHelper.Run(SelectedDevice.Device);
		}

		bool CanBindMs()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.DriverType == DriverType.MS_1 || SelectedDevice.Device.Driver.DriverType == DriverType.MS_2));
		}

		public RelayCommand<bool> ExecuteCustomAdminFunctionsCommand { get; private set; }
		void OnExecuteCustomAdminFunctions(bool isUsb)
		{
			DialogService.ShowModalWindow(new CustomAdminFunctionsCommandViewModel(SelectedDevice.Device));
		}

		bool CanExecuteCustomAdminFunctions(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanExecuteCustomAdminFunctions));
		}

		public RelayCommand GetConfigurationParametersCommand { get; private set; }
		void OnGetConfigurationParameters()
		{
			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, "Считывание параметров устройства");
		}
		private OperationResult<List<Property>> result;
		void OnPropgress()
		{
			result = FiresecManager.FiresecDriver.GetConfigurationParameters(SelectedDevice.Device.UID);
		}
		void OnCompleted()
		{
			if (!result.HasError)
			{
				foreach (var resultProperty in result.Result)
				{
					var property = SelectedDevice.Device.Properties.FirstOrDefault(x => x.Name == resultProperty.Name);
					if (property == null)
					{
						property = new Property()
						{
							Name = resultProperty.Name
						};
						SelectedDevice.Device.Properties.Add(property);
					}
					property.Value = resultProperty.Value;
				}
				SelectedDevice.UpdataConfigurationProperties();
				MessageBoxService.Show("Операция завершилась успешно", "Firesec");
			}
			else
			{
				MessageBoxService.Show("При вызове метода на сервере возникло исключение " + result.Error);
			}
		}
		bool CanGetConfigurationParameters()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties));
		}

		public RelayCommand SetConfigurationParametersCommand { get; private set; }
		void OnSetConfigurationParameters()
		{
			bool ewrthg_ok = true;
			foreach (var property in SelectedDevice.Device.Properties)
			{
				var prop = SelectedDevice.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (prop != null &&
					prop.DriverPropertyType == DriverPropertyTypeEnum.IntType &&
					(int.Parse(property.Value) < prop.Min || int.Parse(property.Value) > prop.Max)
					)
				{
					MessageBoxService.Show("Значение параметра " + prop.Caption + " вне допустимого диапазона", "Firesec");
					ewrthg_ok = false;
				}
			}


			if (ewrthg_ok)
				ServiceFactory.ProgressService.Run(OnPropgress1, OnCompleted1, "Запись параметров в устройство");
		}
		void OnPropgress1()
		{
			FiresecManager.FiresecDriver.SetConfigurationParameters(SelectedDevice.Device.UID,
																		SelectedDevice.Device.Properties);
		}
		void OnCompleted1()
		{
			MessageBoxService.Show("Операция завершилась успешно", "Firesec");
		}
		bool CanSetConfigurationParameters()
		{
			
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties));
		}

		public bool IsAlternativeUSB
		{
			get
			{
				return  ((SelectedDevice != null) && (SelectedDevice.Device.Driver.IsAlternativeUSB));
			}
		}
	}
}