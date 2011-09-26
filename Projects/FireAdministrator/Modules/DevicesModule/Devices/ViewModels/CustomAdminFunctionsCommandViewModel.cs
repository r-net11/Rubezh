using Infrastructure.Common;
using System;
using FiresecClient;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace DevicesModule.ViewModels
{
    public class CustomAdminFunctionsCommandViewModel : SaveCancelDialogContent
    {
        Device _device;
        public CustomAdminFunctionsCommandViewModel(Device device)
        {
            _device = device;
            Title = "Выбор функции";

            Functions = FiresecManager.DeviceCustomFunctionList(device.Driver.UID);
        }

        public List<DeviceCustomFunction> Functions { get; private set; }

        DeviceCustomFunction _selectedFunction;
        public DeviceCustomFunction SelectedFunction
        {
            get { return _selectedFunction; }
            set
            {
                _selectedFunction = value;
                OnPropertyChanged("SelectedFunction");
            }
        }

        protected override void Save(ref bool cancel)
        {
            if (SelectedFunction != null)
            {
                var code = SelectedFunction.Code;
                CustomAdminFunctionsHelper.Run(_device, code);
            }
        }
    }
}
