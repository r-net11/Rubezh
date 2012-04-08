using FiresecClient;
using Infrastructure;
using Controls.MessageBox;

namespace DevicesModule.ViewModels
{
    public static class WriteAllDeviceConfigurationHelper
    {
        static string _result;

        public static void Run()
        {
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, "Запись конфигурации во все устройства");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.WriteAllDeviceConfiguration();
        }

        static void OnCompleted()
        {
            if (string.IsNullOrEmpty(_result))
                MessageBoxService.Show("Операция закончилась успешно");
            else
                MessageBoxService.Show(_result);
        }
    }
}