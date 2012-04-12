using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Controls.MessageBox;

namespace DevicesModule.ViewModels
{
    public class CustomAdminFunctionsCommandViewModel : SaveCancelDialogContent
    {
        Device _device;
        public CustomAdminFunctionsCommandViewModel(Device device)
        {
            _device = device;
            Title = "Выбор функции";

            var operationResult = FiresecManager.FiresecService.DeviceCustomFunctionList(device.Driver.UID);
            if (operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", operationResult.Error);
                return;
            }
            Functions = operationResult.Result;
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
                DeviceCustomFunctionExecuteHelper.Run(_device, SelectedFunction.Code);
            }
        }
    }
}