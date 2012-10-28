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
				((MenuViewModel)e.NewValue).SetNewConfigEvent += (s, ee) => { ee.Cancel = !ConfigManager.SetNewConfig(); };
        }

        void SaveService_Changed()
        {
            _saveButton.IsEnabled = ServiceFactory.SaveService.HasChanges;
        }

        void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            if (MessageBoxService.ShowQuestion("Вы уверены, что хотите перезаписать текущую конфигурацию?") == MessageBoxResult.Yes)
            {
				if (CanChangeConfig == false)
				{
					MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
				}

				ConfigManager.SetNewConfig();
            }
        }

        void OnCreateNew(object sender, RoutedEventArgs e)
        {
			ConfigManager.CreateNew();
        }

        public bool CanChangeConfig
        {
            get { return (FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig)); }
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