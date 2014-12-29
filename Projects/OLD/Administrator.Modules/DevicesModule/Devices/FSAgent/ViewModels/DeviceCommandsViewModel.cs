using System.ComponentModel;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel DevicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			AutoDetectCommand = new RelayCommand(OnAutoDetect, CanAutoDetect);
			ReadDeviceCommand = new RelayCommand<bool>(OnReadDevice, CanReadDevice);
			WriteDeviceCommand = new RelayCommand<bool>(OnWriteDevice, CanWriteDevice);
			WriteAllDeviceCommand = new RelayCommand(OnWriteAllDevice, CanWriteAllDevice);
			SynchronizeDeviceCommand = new RelayCommand<bool>(OnSynchronizeDevice, CanSynchronizeDevice);
			UpdateSoftCommand = new RelayCommand<bool>(OnUpdateSoft, CanUpdateSoft);
			GetDescriptionCommand = new RelayCommand<bool>(OnGetDescription, CanGetDescription);
			GetDeviceJournalCommand = new RelayCommand<bool>(OnGetDeviceJournal, CanGetDeviceJournal);
			SetPasswordCommand = new RelayCommand<bool>(OnSetPassword, CanSetPassword);
			BindMsCommand = new RelayCommand(OnBindMs, CanBindMs);
			ExecuteCustomAdminFunctionsCommand = new RelayCommand<bool>(OnExecuteCustomAdminFunctions, CanExecuteCustomAdminFunctions);
			ConvertCommand = new RelayCommand(OnConvert);

			DevicesViewModel = devicesViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return DevicesViewModel.SelectedDevice; }
		}

		public RelayCommand AutoDetectCommand { get; private set; }
		void OnAutoDetect()
		{
			if (CheckNeedSave())
			{
				AutoDetectDeviceHelper.Run(SelectedDevice);
			}
		}
		bool CanAutoDetect()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.CanAutoDetect &&
				SelectedDevice.Device.Driver.DriverType != DriverType.MS_1 && SelectedDevice.Device.Driver.DriverType != DriverType.MS_2;
		}

		#region ReadWriteDevice
		public RelayCommand<bool> ReadDeviceCommand { get; private set; }
		void OnReadDevice(bool isUsb)
		{
			if (CheckNeedSave())
			{
				DeviceReadConfigurationHelper.Run(SelectedDevice.Device, isUsb);
			}
		}
		bool CanReadDevice(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReadDatabase));
		}

		public RelayCommand<bool> WriteDeviceCommand { get; private set; }
		void OnWriteDevice(bool isUsb)
		{
			if (CheckNeedSave())
			{
				if (ValidateConfiguration())
					DeviceWriteConfigurationHelper.Run(SelectedDevice.Device, isUsb);
			}
		}
		bool CanWriteDevice(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return (SelectedDevice != null && SelectedDevice.Device.Driver.CanWriteDatabase && FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig));
		}

		public RelayCommand WriteAllDeviceCommand { get; private set; }
		void OnWriteAllDevice()
		{
			if (CheckNeedSave())
			{
				if (ValidateConfiguration())
				{
					WriteAllDeviceConfigurationHelper.Run();
				}
			}
		}
		bool CanWriteAllDevice()
		{
			return (FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig));
		}
		#endregion

		public RelayCommand<bool> SynchronizeDeviceCommand { get; private set; }
		void OnSynchronizeDevice(bool isUsb)
		{
			if (CheckNeedSave())
			{
				SynchronizeDeviceHelper.Run(SelectedDevice.Device, isUsb);
			}
		}
		bool CanSynchronizeDevice(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return (SelectedDevice != null && SelectedDevice.Device.Driver.CanSynchonize);
		}

		public RelayCommand<bool> UpdateSoftCommand { get; private set; }
		void OnUpdateSoft(bool isUsb)
		{
			if (ConnectionSettingsManager.IsRemote)
			{
				MessageBoxService.ShowError("Операция обновления ПО доступна только для локального сервера");
				return;
			}

#if DEBUG
			if (FirmwareAllUpdateHelper.IsManyDevicesToUpdate(SelectedDevice.Device))
			{
				if (MessageBoxService.ShowQuestion("Обновить ПО во всех устройствах этого типа?"))
				{
					FirmwareAllUpdateHelper.Run(SelectedDevice.Device);
				}
				return;
			}
#endif

			FirmwareUpdateHelper.Run(SelectedDevice.Device, isUsb);
		}
		bool CanUpdateSoft(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return (SelectedDevice != null && SelectedDevice.Device.Driver.CanUpdateSoft && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
		}

		public RelayCommand<bool> GetDescriptionCommand { get; private set; }
		void OnGetDescription(bool isUsb)
		{
			if (CheckNeedSave())
			{
				DeviceGetInformationHelper.Run(SelectedDevice.Device, isUsb);
			}
		}
		bool CanGetDescription(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanGetDescription));
		}

		public RelayCommand<bool> GetDeviceJournalCommand { get; private set; }
		void OnGetDeviceJournal(bool isUsb)
		{
			if (CheckNeedSave())
			{
				ReadDeviceJournalHelper.Run(SelectedDevice.Device, isUsb);
			}
		}
		bool CanGetDeviceJournal(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return (SelectedDevice != null && SelectedDevice.Device.Driver.CanReadJournal);
		}

		public RelayCommand<bool> SetPasswordCommand { get; private set; }
		void OnSetPassword(bool isUsb)
		{
			if (CheckNeedSave())
			{
				DialogService.ShowModalWindow(new SetPasswordViewModel(SelectedDevice.Device, isUsb));
			}
		}
		bool CanSetPassword(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return (SelectedDevice != null && SelectedDevice.Device.Driver.CanSetPassword);
		}

		public RelayCommand BindMsCommand { get; private set; }
		void OnBindMs()
		{
			if (CheckNeedSave())
			{
				DeviceGetSerialListHelper.Run(SelectedDevice.Device);
			}
		}
		bool CanBindMs()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.DriverType == DriverType.MS_1 || SelectedDevice.Device.Driver.DriverType == DriverType.MS_2));
		}

		public RelayCommand<bool> ExecuteCustomAdminFunctionsCommand { get; private set; }
		void OnExecuteCustomAdminFunctions(bool isUsb)
		{
			if (CheckNeedSave())
			{
				DialogService.ShowModalWindow(new CustomAdminFunctionsCommandViewModel(SelectedDevice.Device, isUsb));
			}
		}
		bool CanExecuteCustomAdminFunctions(bool isUsb)
		{
			if (isUsb && !IsAlternativeUSB)
				return false;
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.DriverType == DriverType.IndicationBlock ||
				SelectedDevice.Device.Driver.DriverType == DriverType.PDU ||
				SelectedDevice.Device.Driver.DriverType == DriverType.PDU_PT));
		}

		public RelayCommand ConvertCommand { get; private set; }
		void OnConvert()
		{
			var fs1ConvertationHelper = new FS1ConvertationHelper();
			fs1ConvertationHelper.ConvertConfiguration();
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.CannotSave(ModuleType.Devices) || validationResult.CannotWrite(ModuleType.Devices))
			{
				MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
				return false;
			}
			if (validationResult.HasErrors(ModuleType.Devices))
			{
				if (!MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить"))
					return false;
			}
			return true;
		}

		public bool IsAlternativeUSB
		{
			get
			{
				var result = (SelectedDevice != null && SelectedDevice.Device.Driver.IsAlternativeUSB && SelectedDevice.Parent != null && SelectedDevice.Parent.Driver.DriverType != DriverType.Computer && SelectedDevice.Parent.Driver.DriverType != DriverType.ComPort_V2);
				return result;
			}
		}

		bool CheckNeedSave()
		{
			if (ServiceFactory.SaveService.FSChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?"))
				{
					var cancelEventArgs = new CancelEventArgs();
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs);
					return !cancelEventArgs.Cancel;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		public bool IsFS2Enabled
		{
			//get { return FiresecManager.IsFS2Enabled; }
			get { return false; }
		}

		public RelayCommand ReadJournalFromFileCommand { get; private set; }
	}
}