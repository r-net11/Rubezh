using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Diagnostics;
using FS2Api;

namespace DevicesModule.ViewModels
{
    public class FS2DeviceCommandsViewModel : BaseViewModel
    {
        DevicesViewModel DevicesViewModel;
        public DeviceCommandsAuParametersViewModel DeviceCommandsAuParametersViewModel { get; private set; }

		public FS2DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
        {
            AutoDetectCommand = new RelayCommand(OnAutoDetect, CanAutoDetect);
            ReadDeviceCommand = new RelayCommand<bool>(OnReadDevice, CanReadDevice);
            WriteDeviceCommand = new RelayCommand<bool>(OnWriteDevice, CanWriteDevice);
            WriteAllDeviceCommand = new RelayCommand(OnWriteAllDevice, CanWriteAllDevice);
            SynchronizeDeviceCommand = new RelayCommand<bool>(OnSynchronizeDevice, CanSynchronizeDevice);
            UpdateSoftCommand = new RelayCommand<bool>(OnUpdateSoft, CanUpdateSoft);
            GetDescriptionCommand = new RelayCommand<bool>(OnGetDescription, CanGetDescription);
            GetDeveceJournalCommand = new RelayCommand<bool>(OnGetDeveceJournal, CanGetDeveceJournal);
            SetPasswordCommand = new RelayCommand<bool>(OnSetPassword, CanSetPassword);
            BindMsCommand = new RelayCommand(OnBindMs, CanBindMs);
            ExecuteCustomAdminFunctionsCommand = new RelayCommand<bool>(OnExecuteCustomAdminFunctions, CanExecuteCustomAdminFunctions);
			MergeConfigurationCommand = new RelayCommand(OnMergeConfiguration, CanMergeConfiguration);

            DevicesViewModel = devicesViewModel;
            DeviceCommandsAuParametersViewModel = new ViewModels.DeviceCommandsAuParametersViewModel(DevicesViewModel);
        }

        public DeviceViewModel SelectedDevice
        {
            get { return DevicesViewModel.SelectedDevice; }
        }

        public RelayCommand AutoDetectCommand { get; private set; }
        void OnAutoDetect()
        {
            AutoDetectDeviceHelper.Run(SelectedDevice);
        }
        bool CanAutoDetect()
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanAutoDetect);
        }

        #region ReadWriteDevice
        public RelayCommand<bool> ReadDeviceCommand { get; private set; }
        void OnReadDevice(bool isUsb)
        {
            FS2DeviceReadConfigurationHelper.Run(SelectedDevice.Device, isUsb);
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
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanWriteDatabase && FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig));
        }

        public RelayCommand WriteAllDeviceCommand { get; private set; }
        void OnWriteAllDevice()
        {
//#if DEBUG
//            if (GlobalSettingsHelper.GlobalSettings.FSAgent_UseFS2)
//            {
//                ClientFS2.ConfigurationManager.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
//                ClientFS2.ConfigurationManager.DriversConfiguration = FiresecManager.FiresecConfiguration.DriversConfiguration;

//                var configurationWriterHelper = new ClientFS2.ConfigurationWriter.ConfigurationWriterHelper();
//                configurationWriterHelper.Run();
//                var configurationDatabaseViewModel = new ClientFS2.ViewModels.ConfigurationDatabaseViewModel(configurationWriterHelper);
//                DialogService.ShowModalWindow(configurationDatabaseViewModel);
//                return;
//            }
//#endif
			if (ValidateConfiguration())
            {
                WriteAllDeviceConfigurationHelper.Run();
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
			FS2SynchronizeDeviceHelper.Run(SelectedDevice.Device, isUsb);

			return;
			FiresecManager.FS2ClientContract.Progress += new System.Action<FS2Api.FS2ProgressInfo>(FS2ClientContract_Progress);
			var result = FiresecManager.FS2ClientContract.DeviceDatetimeSync(SelectedDevice.Device.UID, isUsb);
			if (result.HasError)
			{
				MessageBoxService.ShowError(result.Error, "Ошибка при вызове операции");
			}
        }

		void FS2ClientContract_Progress(FS2ProgressInfo fs2ProgressInfo)
		{
			Trace.WriteLine("FS2ClientContract_Progress " + fs2ProgressInfo.Comment);
		}
        bool CanSynchronizeDevice(bool isUsb)
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanSynchonize);
        }

        public RelayCommand<bool> UpdateSoftCommand { get; private set; }
        void OnUpdateSoft(bool isUsb)
        {
			if (AppSettingsManager.IsRemote)
			{
				MessageBoxService.ShowError("Операция обновления ПО доступна только для локального сервера");
				return;
			}
            FirmwareUpdateHelper.Run(SelectedDevice.Device, isUsb);
        }
        bool CanUpdateSoft(bool isUsb)
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanUpdateSoft && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
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
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanReadJournal);
        }

        public RelayCommand<bool> SetPasswordCommand { get; private set; }
        void OnSetPassword(bool isUsb)
        {
            DialogService.ShowModalWindow(new SetPasswordViewModel(SelectedDevice.Device, isUsb));
        }
        bool CanSetPassword(bool isUsb)
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanSetPassword);
        }

        public RelayCommand BindMsCommand { get; private set; }
        void OnBindMs()
        {
            DeviceGetSerialListHelper.Run(SelectedDevice.Device);
        }
        bool CanBindMs()
        {
            return (SelectedDevice != null && (SelectedDevice.Device.Driver.DriverType == DriverType.MS_1 || SelectedDevice.Device.Driver.DriverType == DriverType.MS_2));
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

        public bool IsAlternativeUSB
        {
            get { return (SelectedDevice != null && SelectedDevice.Device.Driver.IsAlternativeUSB); }
        }

		public RelayCommand MergeConfigurationCommand { get; private set; }
		void OnMergeConfiguration()
		{
			DevicesModuleMerge.MergeConfigurationHelper.Merge();
		}
		bool CanMergeConfiguration()
		{
#if DEBUG
			return true;
#endif
			return false;
		}
    }
}