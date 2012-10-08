using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using Common;
using FireAdministrator.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Microsoft.Win32;
using FiresecAPI;

namespace FireAdministrator.Views
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(MenuView_DataContextChanged);
            ServiceFactory.SaveService.Changed += new Action(SaveService_Changed);
        }

        void MenuView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MenuViewModel)
                ((MenuViewModel)e.NewValue).SetNewConfigEvent += (s, ee) => { ee.Cancel = !SetNewConfig(); };
        }

        void SaveService_Changed()
        {
            _saveButton.IsEnabled = ServiceFactory.SaveService.HasChanges;
        }

        void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            if (MessageBoxService.ShowQuestion("Вы уверены, что хотите перезаписать текущую конфигурацию?") == MessageBoxResult.Yes)
            {
                SetNewConfig();
            }
        }

        public bool SetNewConfig()
        {
            if (CanChangeConfig == false)
            {
                MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
                return false;
            }

            ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Publish(null);

            var validationResult = ServiceFactory.ValidationService.Validate();
            if (validationResult.HasErrors())
            {
                if (validationResult.CannotSave())
                {
                    MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
                    return false;
                }

                if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?") != MessageBoxResult.Yes)
                    return false;
            }

            WaitHelper.Execute(() =>
            {
                LoadingService.ShowProgress("Применение конфигурации", "Применение конфигурации", 10);
                if (ServiceFactory.SaveService.DevicesChanged)
                {
                    LoadingService.DoStep("Применение конфигурации устройств");
					if (!ServiceFactory.AppSettings.DoNotOverrideFS1)
					{
						var fsResult = FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
						if (fsResult.HasError)
						{
							MessageBoxService.ShowError(fsResult.Error);
						}
					}
                    LoadingService.DoStep("Сохранение конфигурации устройств");
                    var result = FiresecManager.FiresecService.SetDeviceConfiguration(FiresecManager.FiresecConfiguration.DeviceConfiguration);
                    if (result.HasError)
                    {
                        MessageBoxService.ShowError(result.Error);
                    }
                }

                if (ServiceFactory.SaveService.PlansChanged)
                {
                    LoadingService.DoStep("Сохранение конфигурации графических планов");
                    FiresecManager.FiresecService.SetPlansConfiguration(FiresecManager.PlansConfiguration);
                }

                if (ServiceFactory.SaveService.SecurityChanged)
                {
                    LoadingService.DoStep("Сохранение конфигурации пользователей и ролей");
                    FiresecManager.FiresecService.SetSecurityConfiguration(FiresecManager.SecurityConfiguration);
                }

                if (ServiceFactory.SaveService.LibraryChanged)
                {
                    LoadingService.DoStep("Сохранение конфигурации библиотеки устройств");
                    FiresecManager.FiresecService.SetLibraryConfiguration(FiresecManager.LibraryConfiguration);
                }

                if ((ServiceFactory.SaveService.InstructionsChanged) ||
                    (ServiceFactory.SaveService.SoundsChanged) ||
                    (ServiceFactory.SaveService.FilterChanged) ||
                    (ServiceFactory.SaveService.CamerasChanged))
                {
                    LoadingService.DoStep("Сохранение конфигурации прочих настроек");
                    FiresecManager.FiresecService.SetSystemConfiguration(FiresecManager.SystemConfiguration);
                }

                if (ServiceFactory.SaveService.XDevicesChanged)
                {
                    LoadingService.DoStep("Сохранение конфигурации ГК");
                    FiresecManager.FiresecService.SetXDeviceConfiguration(XManager.DeviceConfiguration);
                }
            });
            LoadingService.Close();
            ServiceFactory.SaveService.Reset();
            return true;
        }

        void OnCreateNew(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxService.ShowQuestion("Вы уверены, что хотите создать новую конфигурацию");
            if (result == MessageBoxResult.Yes)
            {
				FiresecManager.SetEmptyConfiguration();
				XManager.SetEmptyConfiguration();
                FiresecManager.PlansConfiguration = new PlansConfiguration();
                FiresecManager.SystemConfiguration = new SystemConfiguration();
                FiresecManager.PlansConfiguration.Update();

                ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

                ServiceFactory.SaveService.DevicesChanged = true;
                ServiceFactory.SaveService.PlansChanged = true;
                ServiceFactory.SaveService.InstructionsChanged = true;
                ServiceFactory.SaveService.SoundsChanged = true;
                ServiceFactory.SaveService.FilterChanged = true;
                ServiceFactory.SaveService.CamerasChanged = true;
                ServiceFactory.SaveService.XDevicesChanged = true;

                ServiceFactory.Layout.Close();
                ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
                ServiceFactory.Layout.ShowFooter(null);
            }
        }

        public bool CanChangeConfig
        {
            get { return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ChangeConfigDevices)); }
        }

        void OnValidate(object sender, RoutedEventArgs e)
        {
            ServiceFactory.ValidationService.Validate();
        }

        void OnSaveToFile(object sender, RoutedEventArgs e)
        {
            FileConfigurationHelper.SaveToFile();
        }

        void OnLoadFromFile(object sender, RoutedEventArgs e)
        {
            FileConfigurationHelper.LoadFromFile();
        }
    }
}