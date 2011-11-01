using System.IO;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Microsoft.Win32;

namespace DevicesModule.ViewModels
{
    public static class FirmwareUpdateHelper
    {
        static Device _device;
        static string _fileName;
        static string _question;
        static string _result;
        static byte[] bytes;

        public static void Run(Device device)
        {
            _device = device;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.HXP)|*.HXP|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _fileName = openFileDialog.FileName;
                using (var fileStream = new FileStream(_fileName, FileMode.Open))
                {
                    bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                }

                _question = FiresecManager.DeviceVerifyFirmwareVersion(_device.UID, bytes, new FileInfo(_fileName).Name);
                FiresecManager.DeviceUpdateFirmware(_device.UID, bytes, new FileInfo(_fileName).Name);
                return;

                var asyncInstanceOperationHelper = new AsyncInstanceOperationHelper();
                asyncInstanceOperationHelper.Run(OnVarifyPropgress, OnVerifyCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
            }
        }

        static void OnVarifyPropgress()
        {
            _question = FiresecManager.DeviceVerifyFirmwareVersion(_device.UID, bytes, new FileInfo(_fileName).Name);
        }

        static void OnVerifyCompleted()
        {
            if (DialogBox.DialogBox.Show(_question, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var asyncInstanceOperationHelper = new AsyncInstanceOperationHelper();
                asyncInstanceOperationHelper.Run(OnUpdatePropgress, OnUpdateCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
            }
        }

        static void OnUpdatePropgress()
        {
            _result = FiresecManager.DeviceUpdateFirmware(_device.UID, bytes, new FileInfo(_fileName).Name);
        }

        static void OnUpdateCompleted()
        {
            var result = _result;
            //DialogBox.DialogBox.Show(_result);
        }
    }
}