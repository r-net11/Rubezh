using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Microsoft.Win32;
using System.IO;
using FiresecClient;
using System.Windows;

namespace DevicesModule.ViewModels
{
    public static class FirmwareUpdateHelper
    {
        static Device _device;
        static Stream _fileStream;
        static string _question;

        public static void Run(Device device)
        {
            _device = device;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.HXP)|*.HXP|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _fileStream = new FileStream(openFileDialog.FileName, FileMode.Open);
                AsyncOperationHelper.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
            }
        }

        static void OnPropgress()
        {
            byte[] bytes = new byte[_fileStream.Length];
            _fileStream.Read(bytes, 0, bytes.Length);
            _fileStream.Close();
            _question = FiresecManager.DeviceVerifyFirmwareVersion(_device.UID, bytes);
        }

        static void OnCompleted()
        {
            MessageBox.Show(_question);
        }
    }
}
