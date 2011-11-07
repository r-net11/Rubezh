using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DeviceCommandsViewModel : BaseViewModel
    {
        DevicesViewModel _devicesViewModel;

        public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
        {
            _devicesViewModel = devicesViewModel;

            AutoDetectCommand = new RelayCommand(OnAutoDetect, CanAutoDetect);
            ReadDeviceCommand = new RelayCommand<bool>(OnReadDevice, CanReadDevice);
            WriteDeviceCommand = new RelayCommand<bool>(OnWriteDevice, CanWriteDevice);
            WriteAllDeviceCommand = new RelayCommand(OnWriteAllDevice, CanWriteAllDevice);
            SynchronizeDeviceCommand = new RelayCommand<bool>(OnSynchronizeDevice, CanSynchronizeDevice);
            RebootDeviceCommand = new RelayCommand(OnRebootDevice, CanRebootDevice);
            UpdateSoftCommand = new RelayCommand<bool>(OnUpdateSoft, CanUpdateSoft);
            GetDescriptionCommand = new RelayCommand<bool>(OnGetDescription, CanGetDescription);
            GetDeveceJournalCommand = new RelayCommand<bool>(OnGetDeveceJournal, CanGetDeveceJournal);
            SetPasswordCommand = new RelayCommand<bool>(OnSetPassword, CanSetPassword);
            BindMsCommand = new RelayCommand(OnBindMs, CanBindMs);
            ExecuteCustomAdminFunctionsCommand = new RelayCommand<bool>(OnExecuteCustomAdminFunctions, CanExecuteCustomAdminFunctions);
        }

        public DeviceViewModel SelectedDevice
        {
            get { return _devicesViewModel.SelectedDevice; }
        }

        public RelayCommand AutoDetectCommand { get; private set; }
        void OnAutoDetect()
        {
            AutoSearchHelper.Run(SelectedDevice);
        }

        bool CanAutoDetect()
        {
            return true;
            //return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanAutoDetect));
        }

        public RelayCommand<bool> ReadDeviceCommand { get; private set; }
        void OnReadDevice(bool isUsb)
        {
            FiresecManager.DeviceReadConfiguration(SelectedDevice.Device.UID, isUsb);
        }

        bool CanReadDevice(bool isUsb)
        {
            return false;
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReadDatabase));
        }

        public RelayCommand<bool> WriteDeviceCommand { get; private set; }
        void OnWriteDevice(bool isUsb)
        {
            FiresecManager.DeviceWriteConfiguration(SelectedDevice.Device.UID);
        }

        bool CanWriteDevice(bool isUsb)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanWriteDatabase));
        }

        public RelayCommand WriteAllDeviceCommand { get; private set; }
        void OnWriteAllDevice()
        {
            FiresecManager.WriteAllDeviceConfiguration();
        }

        bool CanWriteAllDevice()
        {
            return true;
        }

        public RelayCommand<bool> SynchronizeDeviceCommand { get; private set; }
        void OnSynchronizeDevice(bool isUsb)
        {
            DeviceSynchrinizationHelper.Run(SelectedDevice.Device.UID);
        }

        bool CanSynchronizeDevice(bool isUsb)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSynchonize));
        }

        public RelayCommand RebootDeviceCommand { get; private set; }
        void OnRebootDevice()
        {
            FiresecManager.DeviceRestart(SelectedDevice.Device.UID);
        }

        bool CanRebootDevice()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReboot));
        }

        public RelayCommand<bool> UpdateSoftCommand { get; private set; }
        void OnUpdateSoft(bool isUsb)
        {
            FirmwareUpdateHelper.Run(SelectedDevice.Device);
        }

        bool CanUpdateSoft(bool isUsb)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanUpdateSoft));
        }

        public RelayCommand<bool> GetDescriptionCommand { get; private set; }
        void OnGetDescription(bool isUsb)
        {
            GetInformationHelper.Run(SelectedDevice.Device);
        }

        bool CanGetDescription(bool isUsb)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanGetDescription));
        }

        public RelayCommand<bool> GetDeveceJournalCommand { get; private set; }
        void OnGetDeveceJournal(bool isUsb)
        {
            DeviceJournalHelper.Run(SelectedDevice.Device);
        }

        bool CanGetDeveceJournal(bool isUsb)
        {
            return true;
        }

        public RelayCommand<bool> SetPasswordCommand { get; private set; }
        void OnSetPassword(bool isUsb)
        {
            ServiceFactory.UserDialogs.ShowModalWindow(new SetPasswordViewModel(SelectedDevice.Device.UID));
        }

        bool CanSetPassword(bool isUsb)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSetPassword));
        }

        public RelayCommand BindMsCommand { get; private set; }
        void OnBindMs()
        {
            GetSerialsHelper.Run(SelectedDevice.Device);
        }

        bool CanBindMs()
        {
            return true;
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.DriverType == (DriverType.MS_1 | DriverType.MS_2)));
        }

        public RelayCommand<bool> ExecuteCustomAdminFunctionsCommand { get; private set; }
        void OnExecuteCustomAdminFunctions(bool isUsb)
        {
            ServiceFactory.UserDialogs.ShowModalWindow(new CustomAdminFunctionsCommandViewModel(SelectedDevice.Device));
        }

        bool CanExecuteCustomAdminFunctions(bool isUsb)
        {
            return true;
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanExecuteCustomAdminFunctions));
        }

        public bool IsAlternativeUSB
        {
            get
            {
                return true;
                return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.IsAlternativeUSB));
            }
        }
    }
}