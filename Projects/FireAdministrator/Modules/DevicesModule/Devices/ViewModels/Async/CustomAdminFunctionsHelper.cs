using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using System.Windows;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public static class CustomAdminFunctionsHelper
    {
        static Device _device;
        static string _functionCode;
        static string _result;

        public static void Run(Device device, string functionCode)
        {
            _device = device;
            _functionCode = functionCode;

            AsyncOperationHelper.Run(OnPropgress, OnlCompleted, device.PresentationAddressDriver + ". Выполнение функции");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.DeviceCustomFunctionExecute(_device.UID, _functionCode);
        }

        static void OnlCompleted()
        {
            MessageBox.Show(_result);
        }
    }
}
