using System;
using System.Linq;
using System.Windows;
using FireMonitor.ViewModels;
using Firesec;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Common.Theme;
using Common.GK;
using Microsoft.Win32;

namespace FireMonitor
{
    public class Bootstrapper : BaseBootstrapper
    {
        public void Initialize()
        {
            AppConfigHelper.InitializeAppSettings();
            ThemeHelper.LoadThemeFromRegister();
            VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
            ServiceFactory.Initialize(new LayoutService(), new SecurityService());
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

            if (ServiceFactory.LoginService.ExecuteConnect())
                try
                {
                    LoadingService.Show("Чтение конфигурации", 15);
                    LoadingService.AddCount(GetModuleCount());

                    LoadingService.DoStep("Синхронизация файлов");
                    FiresecManager.UpdateFiles();
                    InitializeFs(false);
                    var loadingError = FiresecManager.GetLoadingError();
					if (!String.IsNullOrEmpty(loadingError))
					{
						MessageBoxService.ShowWarning(loadingError, "Ошибки при загрузке драйвера FireSec");
					}
                    LoadingService.DoStep("Загрузка конфигурации ГК");
                    InitializeGk();

                    LoadingService.DoStep("Старт полинга сервера");
                    FiresecManager.StartPoll(false);
#if RELEASE
                    LoadingService.DoStep("Проверка HASP-ключа");
                    var operationResult = FiresecManager.FiresecDriver.CheckHaspPresence();
                    if (operationResult.HasError)
                        MessageBoxService.ShowWarning("HASP-ключ на сервере не обнаружен. Время работы приложения будет ограничено");
#endif
                    LoadingService.DoStep("Проверка прав пользователя");
                    if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login))
                    {
                        LoadingService.DoStep("Загрузка клиентских настроек");
                        ClientSettings.LoadSettings();

                        var shell = new MonitorShellViewModel();
                        ((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
                        ((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
                        RunShell(shell);
                        ((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
                        ((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());

                        SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
                        ServiceFactory.Events.GetEvent<NotifyEvent>().Subscribe(OnNotify);
                    }
                    else
                    {
                        MessageBoxService.Show("Нет прав на работу с программой");
                        FiresecManager.Disconnect();
                    }
                    LoadingService.Close();
                }
                catch (Exception ex)
                {
                    MessageBoxService.ShowException(ex);
                    Application.Current.Shutdown();
                }
            else
                Application.Current.Shutdown();
            //MutexHelper.KeepAlive();
            ServiceFactory.SubscribeEvents();
        }

        void InitializeFs(bool reconnect = false)
        {
            LoadingService.DoStep("Загрузка конфигурации с сервера");
            FiresecManager.GetConfiguration();

            if (!reconnect)
            {
                LoadingService.DoStep("Инициализация драйвера устройств");
                FiresecManager.InitializeFiresecDriver(ServiceFactory.AppSettings.FS_Address, ServiceFactory.AppSettings.FS_Port, ServiceFactory.AppSettings.FS_Login, ServiceFactory.AppSettings.FS_Password, true);
            }
            LoadingService.DoStep("Синхронизация конфигурации");
            FiresecManager.FiresecDriver.Synchronyze();
            LoadingService.DoStep("Старт мониторинга");
            FiresecManager.FiresecDriver.StartWatcher(true, true);
            if (!reconnect)
            {
                LoadingService.DoStep("Синхронизация журнала событий");
                FiresecManager.SynchrinizeJournal();
            }
        }

        void InitializeGk()
        {
            GKDriversCreator.Create();
            XManager.GetConfiguration();
            XManager.CreateStates();
            DatabaseManager.Convert();
        }

        void OnConfigurationChanged()
        {
            LoadingService.Show("Перезагрузка конфигурации", 10);
            LoadingService.AddCount(10);

            ApplicationService.CloseAllWindows();
            ServiceFactory.Layout.Close();

            InitializeFs(true);
            InitializeGk();

            InitializeModules();
            ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);

            LoadingService.Close();
        }

        void OnNotify(string message)
        {
            MessageBoxService.Show(message);
        }
    }
}