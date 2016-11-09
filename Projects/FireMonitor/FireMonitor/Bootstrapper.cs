using Common;
using FireMonitor.ViewModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Localization.FireMonitor.Common;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FireMonitor
{
    public class Bootstrapper : BaseBootstrapper
    {
        private string _login;
        private string _password;

        public bool Initialize()
        {
            bool result;
            LoadingErrorManager.Clear();
            AppConfigHelper.InitializeAppSettings();
            ServiceFactory.Initialize(new LayoutService(), new SecurityService(), new UiElementsVisibilityService());
            ServiceFactoryBase.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
            ServiceFactory.StartupService.Show();
            if (ServiceFactory.StartupService.PerformLogin(_login, _password))
            {
                _login = ServiceFactory.StartupService.Login;
                _password = ServiceFactory.StartupService.Password;

                Logger.Info(string.Format("Bootstrapper. Получаем с Сервера тип окружения рабочего стола для пользователя '{0}'", _login));
                var getUserShelTypeResult = FiresecManager.FiresecService.GetUserShellType(ServiceFactory.StartupService.Login);
                if (!getUserShelTypeResult.HasError)
                {
                    try
                    {
                        SetUserShellType(getUserShelTypeResult.Result);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, string.Format("Возникла исключительная ситуация в процессе установки типа оболочки рабочего стола для пользователя '{0}'", _login));
                    }
                }

                var userChangedEventArgs = new UserChangedEventArgs { IsReconnect = false };
                ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Publish(userChangedEventArgs);

                try
                {
                    // При получении от сервера команды на разрыв соединения выводим соответствующее предупреждение и завершаем работу
                    SafeFiresecService.DisconnectClientCommandEvent += (showNotification) =>
                    {
                        if (showNotification)
                            ApplicationService.Invoke(() => MessageBoxService.ShowWarning(CommonResources.Disconnected));
                        ApplicationService.ShutDown();
                    };

                    // При получении от сервера уведомления о смене лицензии выводим соответствующее предупреждение и завершаем работу
                    SafeFiresecService.LicenseChangedEvent += () =>
                    {
                        ApplicationService.Invoke(() => MessageBoxService.ShowWarning(CommonResources.DisconnectedLicense));
                        ApplicationService.ShutDown();
                    };

                    // Получаем данные лицензии с Сервера
                    ServiceFactory.UiElementsVisibilityService.Initialize(FiresecManager.FiresecService.GetLicenseData().Result);

                    CreateModules();

                    ServiceFactory.StartupService.ShowLoading(CommonResources.ReadConfig, 15);
                    ServiceFactory.StartupService.AddCount(GetModuleCount());

                    ServiceFactory.StartupService.DoStep(CommonResources.FileSync);
                    FiresecManager.UpdateFiles();

                    ServiceFactory.StartupService.DoStep(CommonResources.LoadConfig);
                    FiresecManager.GetConfiguration("Monitor/Configuration");

                    BeforeInitialize(true);

                    ServiceFactory.StartupService.DoStep(CommonResources.StartServerPolling);
                    FiresecManager.StartPoll();

                    ServiceFactory.StartupService.DoStep(CommonResources.CheckPermissions);
                    if (FiresecManager.CheckPermission(PermissionType.Oper_Login))
                    {
                        ServiceFactory.StartupService.DoStep(CommonResources.LoadClientSettings);
                        ClientSettings.LoadSettings();

                        Task.Factory.StartNew(LoadImages);

                        result = Run();
                        SafeFiresecService.ConfigurationChangedEvent += () => ApplicationService.Invoke(OnConfigurationChanged);

                        if (result)
                        {
                            AterInitialize();
                        }
                    }
                    else
                    {
                        MessageBoxService.Show(CommonResources.NoPermissions);
                        FiresecManager.Disconnect();

                        if (Application.Current != null)
                            Application.Current.Shutdown();
                        return false;
                    }

                    if (Process.GetCurrentProcess().ProcessName != "StrazhMonitor.vshost")
                    {
                        RegistrySettingsHelper.SetBool("isException", true);
                    }
                    ServiceFactory.StartupService.Close();
                }
                catch (StartupCancellationException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Bootstrapper.InitializeFs");
                    MessageBoxService.ShowException(e);
                    if (Application.Current != null)
                        Application.Current.Shutdown();
                    return false;
                }
            }
            else
            {
                if (Application.Current != null)
                    Application.Current.Shutdown();
                return false;
            }
            return result;
        }

        /// <summary>
        /// Загрузка всех файлов подложек пропусков и копирование их в клиентскую папку Content
        /// </summary>
        private static void LoadImages()
        {
            var images = FiresecManager.FiresecService.UploadPassCardImages().Result; //TODO: Upload file for those organisations wich user have permission
            if (images == null) return;

            foreach (var image in images)
                ServiceFactoryBase.ContentService.AddContent(image.Data, image.UID);
        }

        private void SetUserShellType(ShellType shellType)
        {
            if (UserShellHelper.GetShell() != shellType)
            {
                Logger.Info(string.Format("Bootstrapper. Для пользователя '{0}' устанавливаем тип окружения рабочего стола на '{1}'", _login, shellType));
                UserShellHelper.SetShell(shellType);

                Logger.Info(string.Format("Bootstrapper. Для пользователя '{0}' {1}ограничиваем доступ к диспетчеру задач", _login, shellType == ShellType.Default ? "не " : string.Empty));
                UserShellHelper.DisableTaskManager(shellType != ShellType.Default);

                MessageBoxService.ShowWarning(CommonResources.RestartOT);
                ApplicationService.ShutDown();

                UserShellHelper.Reboot();
            }
        }

        protected virtual bool Run()
        {

            var result = true;
            var shell = CreateShell();
            ((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
            ((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
            if (!RunShell(shell))
                result = false;
            return result;
        }
        protected virtual ShellViewModel CreateShell()
        {
            return new MonitorShellViewModel();
        }

        protected virtual void OnConfigurationChanged()
        {
            var restartView = new RestartApplicationViewModel();
            var isRestart = DialogService.ShowModalWindow(restartView);
            if (isRestart)
                Restart();
            else
            {
                var timer = new DispatcherTimer();
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    Restart();
                };
                timer.Interval = TimeSpan.FromSeconds(restartView.Total);
                timer.Start();
            }
        }
        private void Restart()
        {
            ApplicationService.ApplicationWindow.IsEnabled = false;
            ServiceFactoryBase.ContentService.Clear();
            FiresecManager.FiresecService.StopPoll();
            LoadingErrorManager.Clear();
            ApplicationService.CloseAllWindows();
            ServiceFactory.Layout.Close();
            ApplicationService.ShutDown();
            RestartApplication();
        }

        public void RestartApplication()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Application.ResourceAssembly.Location,
                Arguments = GetRestartCommandLineArguments()
            };
            Process.Start(processStartInfo);
        }
        protected virtual string GetRestartCommandLineArguments()
        {
            string commandLineArguments = null;
            if (_login != null && _password != null)
                commandLineArguments = "login='" + _login + "' password='" + _password + "'";
            return commandLineArguments;
        }
        public virtual void InitializeCommandLineArguments(string[] args)
        {
            if (args == null || args.Count() < 2) return;

            foreach (var arg in args)
            {
                if (arg.StartsWith("login='") && arg.EndsWith("'"))
                {
                    _login = arg.Replace("login='", "");
                    _login = _login.Replace("'", "");
                }

                if (arg.StartsWith("password='") && arg.EndsWith("'"))
                {
                    _password = arg.Replace("password='", "");
                    _password = _password.Replace("'", "");
                }
            }
        }
    }
}