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

        public static void Run(Device device)
        {
            _device = device;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.HXP)|*.HXP|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _fileName = openFileDialog.FileName;
                AsyncOperationHelper.Run(OnVarifyPropgress, OnVerifyCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
            }
        }

        static void OnVarifyPropgress()
        {
            var fileStream = new FileStream(_fileName, FileMode.Open);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            _question = FiresecManager.DeviceVerifyFirmwareVersion(_device.UID, bytes, new FileInfo(_fileName).Name);
        }

        static void OnVerifyCompleted()
        {
            var result = DialogBox.DialogBox.Show(_question, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                AsyncOperationHelper.Run(OnUpdatePropgress, OnUpdateCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
            }
        }

        static void OnUpdatePropgress()
        {
            var fileStream = new FileStream(_fileName, FileMode.Open);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            _result = FiresecManager.DeviceUpdateFirmware(_device.UID, bytes, new FileInfo(_fileName).Name);
        }

        static void OnUpdateCompleted()
        {
            var result = _result;
            //DialogBox.DialogBox.Show(_result);
        }
    }
}