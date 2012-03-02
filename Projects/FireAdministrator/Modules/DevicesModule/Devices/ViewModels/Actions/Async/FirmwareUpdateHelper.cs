using System.IO;
using System.Windows;
using Controls.MessageBox;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Microsoft.Win32;

namespace DevicesModule.ViewModels
{
    public static class FirmwareUpdateHelper
    {
        static Device _device;
        static bool _isUsb;
        static string _fileName;
        static string _question;
        static string _result;
        static byte[] bytes;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;

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

                ServiceFactory.ProgressService.Run(OnVarifyPropgress, OnVerifyCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
            }
        }

        static void OnVarifyPropgress()
        {
            _question = FiresecManager.DeviceVerifyFirmwareVersion(_device.UID, _isUsb, bytes, new FileInfo(_fileName).Name);
        }

        static void OnVerifyCompleted()
        {
            if (MessageBoxService.ShowQuestion(_question) == MessageBoxResult.Yes)
            {
                ServiceFactory.ProgressService.Run(OnUpdatePropgress, OnUpdateCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
            }
        }

        static void OnUpdatePropgress()
        {
            _result = FiresecManager.DeviceUpdateFirmware(_device.UID, _isUsb, bytes, new FileInfo(_fileName).Name);
        }

        static void OnUpdateCompleted()
        {
            if (string.IsNullOrEmpty(_result))
            MessageBoxService.Show(_result);
        }
    }
}