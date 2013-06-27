using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Diagnostics;
using FS2Api;
using System.IO;
using Microsoft.Win32;
using System.Runtime.Serialization;

namespace DevicesModule.ViewModels
{
    public class FS2DeviceCommandsViewModel : BaseViewModel
    {
        DevicesViewModel DevicesViewModel;
        public FS2DeviceCommandsAuParametersViewModel DeviceCommandsAuParametersViewModel { get; private set; }

		public FS2DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
        {
			ServiceFactory.FS2ProgressService = new FS2ProgressService();
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
			ReadJournalFromFileCommand = new RelayCommand(OnReadJournalFromFile);
			MergeConfigurationCommand = new RelayCommand(OnMergeConfiguration, CanMergeConfiguration);

            DevicesViewModel = devicesViewModel;
            DeviceCommandsAuParametersViewModel = new FS2DeviceCommandsAuParametersViewModel(DevicesViewModel);
        }

        public DeviceViewModel SelectedDevice
        {
            get { return DevicesViewModel.SelectedDevice; }
        }

        public RelayCommand AutoDetectCommand { get; private set; }
        void OnAutoDetect()
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
            FS2AutoDetectDeviceHelper.Run(SelectedDevice.Device);
        }
        bool CanAutoDetect()
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanAutoDetect);
        }

        #region ReadWriteDevice
        public RelayCommand<bool> ReadDeviceCommand { get; private set; }
        void OnReadDevice(bool isUsb)
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
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
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
            if (ValidateConfiguration())
                FS2DeviceWriteConfigurationHelper.Run(SelectedDevice.Device, isUsb);
        }
        bool CanWriteDevice(bool isUsb)
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanWriteDatabase && FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig));
        }

        public RelayCommand WriteAllDeviceCommand { get; private set; }
        void OnWriteAllDevice()
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			if (ValidateConfiguration())
            {
                FS2WriteAllDeviceConfigurationHelper.Run();
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
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			FS2SynchronizeDeviceHelper.Run(SelectedDevice.Device, isUsb);
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
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			if (ConnectionSettingsManager.IsRemote)
			{
				MessageBoxService.ShowError("Операция обновления ПО доступна только для локального сервера");
				return;
			}
            FS2FirmwareUpdateHelper.Run(SelectedDevice.Device, isUsb);
        }
        bool CanUpdateSoft(bool isUsb)
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanUpdateSoft && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
        }

        public RelayCommand<bool> GetDescriptionCommand { get; private set; }
        void OnGetDescription(bool isUsb)
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			FS2DeviceGetInformationHelper.Run(SelectedDevice.Device, isUsb);
        }
        bool CanGetDescription(bool isUsb)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanGetDescription));
        }

        public RelayCommand<bool> GetDeveceJournalCommand { get; private set; }
        void OnGetDeveceJournal(bool isUsb)
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			FS2ReadDeviceJournalHelper.Run(SelectedDevice.Device, isUsb);
        }
        bool CanGetDeveceJournal(bool isUsb)
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanReadJournal);
        }

        public RelayCommand<bool> SetPasswordCommand { get; private set; }
        void OnSetPassword(bool isUsb)
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			DialogService.ShowModalWindow(new FS2SetPasswordViewModel(SelectedDevice.Device, isUsb));
        }
        bool CanSetPassword(bool isUsb)
        {
            return (SelectedDevice != null && SelectedDevice.Device.Driver.CanSetPassword);
        }

        public RelayCommand BindMsCommand { get; private set; }
        void OnBindMs()
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			FS2DeviceGetSerialListHelper.Run(SelectedDevice.Device);
        }
        bool CanBindMs()
        {
            return (SelectedDevice != null && (SelectedDevice.Device.Driver.DriverType == DriverType.MS_1 || SelectedDevice.Device.Driver.DriverType == DriverType.MS_2));
        }

        public RelayCommand<bool> ExecuteCustomAdminFunctionsCommand { get; private set; }
        void OnExecuteCustomAdminFunctions(bool isUsb)
        {
			if (ServiceFactory.SaveService.FSChanged)
			{
				MessageBoxService.Show("Для выполнения этой операции необходимо применить конфигурацию");
				return;
			}
			DialogService.ShowModalWindow(new FS2CustomAdminFunctionsCommandViewModel(SelectedDevice.Device, isUsb));
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

		public RelayCommand ReadJournalFromFileCommand { get; private set; }
		void OnReadJournalFromFile()
		{
			var openDialog = new OpenFileDialog()
			{
				Filter = "firesec2 journal files|*.fscf",
				DefaultExt = "firesec2 journal files|*.fscf"
			};
			if (openDialog.ShowDialog().Value)
			{
				using (var fileStream = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(FS2JournalItemsCollection));
					var fs2JournalItemsCollection = (FS2JournalItemsCollection)dataContractSerializer.ReadObject(fileStream);
					if (fs2JournalItemsCollection != null)
					{
						DialogService.ShowModalWindow(new FS2DeviceJournalViewModel(fs2JournalItemsCollection));
					}
				}
			}
		}

		public bool IsFS2Enabled
		{
			get { return FiresecManager.IsFS2Enabled; }
		}
    }
}