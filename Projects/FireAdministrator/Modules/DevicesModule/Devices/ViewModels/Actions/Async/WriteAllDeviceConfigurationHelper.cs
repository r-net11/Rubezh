using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class WriteAllDeviceConfigurationHelper
    {
        public static void Run()
        {
            ServiceFactory.ProgressService.Run(OnPropgress, null, "Запись конфигурации во все устройства");
        }

        static void OnPropgress()
        {
            FiresecManager.WriteAllDeviceConfiguration();
        }
    }
}
