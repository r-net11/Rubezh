using FiresecClient;
using Infrastructure;
using Controls.MessageBox;
using FiresecAPI;

namespace DevicesModule.ViewModels
{
    public static class WriteAllDeviceConfigurationHelper
    {
        static OperationResult<bool> _operationResult;

        public static void Run()
        {
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, "Запись конфигурации во все устройства");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.WriteAllDeviceConfiguration();
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}