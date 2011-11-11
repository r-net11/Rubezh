using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class WriteAllDeviceConfigurationHelper
    {
        public static void Run()
        {
            AsyncOperationHelper.Run(OnPropgress, null, "Запись конфигурации во все устройства");
        }

        static void OnPropgress()
        {
            FiresecManager.WriteAllDeviceConfiguration();
        }
    }
}
