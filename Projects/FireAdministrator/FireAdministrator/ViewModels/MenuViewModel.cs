using System;
using System.ComponentModel;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace FireAdministrator.ViewModels
{
	public class MenuViewModel : BaseViewModel
	{
		string FileName;
		bool HasChanges = true;

		public MenuViewModel()
		{
			LoadFromFileCommand = new RelayCommand(OnLoadFromFile);
			SaveCommand = new RelayCommand(OnSave, CanSave);
			SaveAsCommand = new RelayCommand(OnSaveAs);
			SaveAllCommand = new RelayCommand(OnSaveAll);
			CreateNewCommand = new RelayCommand(OnCreateNew);
			ValidateCommand = new RelayCommand(OnValidate);
			SetNewConfigCommand = new RelayCommand(OnSetNewConfig, CanSetNewConfig);
			SetPnanNameToZoneDescriptionsCommand = new RelayCommand(OnSetPnanNameToZoneDescriptions);
			MergeConfigurationCommand = new RelayCommand(OnMergeConfiguration);
			ServiceFactory.SaveService.Changed += new Action(SaveService_Changed);
			ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Subscribe(OnSetNewConfiguration);
			IsMainMenuVisible = RegistrySettingsHelper.GetBool("Administrato.Shell.IsMainMenuVisible", false);
			IsMenuVisible = RegistrySettingsHelper.GetBool("Administrato.Shell.IsMenuVisible", true);
		}

		void OnSetNewConfiguration(CancelEventArgs e)
		{
			if (!FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
				MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
				e.Cancel = true;
				return;
			}
			e.Cancel = !ConfigManager.SetNewConfig();
		}

		BaseViewModel _extendedMenu;
		public BaseViewModel ExtendedMenu
		{
			get { return _extendedMenu; }
			set
			{
				_extendedMenu = value;
				OnPropertyChanged(() => ExtendedMenu);
			}
		}

		public bool SetNewConfig()
		{
			if (!FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
				MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
				return false;
			}
			return ConfigManager.SetNewConfig();
		}

		public bool CanShowMainMenu
		{
			get { return false; }
		}

		void SaveService_Changed()
		{
			HasChanges = ServiceFactory.SaveService.HasChanges;
		}

		public RelayCommand LoadFromFileCommand { get; private set; }
		void OnLoadFromFile()
		{
			var fileName = FileConfigurationHelper.LoadFromFile();
			if (!string.IsNullOrEmpty(fileName))
				FileName = fileName;
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			ServiceFactory.Events.GetEvent<BeforeConfigurationSerializeEvent>().Publish(null);
			if (string.IsNullOrEmpty(FileName))
			{
				OnSaveAs();
			}
			else
			{
				WaitHelper.Execute(() =>
				{
					FileConfigurationHelper.SaveToZipFile(FileName);
				});
			}
		}
		bool CanSave()
		{
			return HasChanges;
		}

		public RelayCommand SaveAsCommand { get; private set; }
		void OnSaveAs()
		{
			ServiceFactory.Events.GetEvent<BeforeConfigurationSerializeEvent>().Publish(null);
			WaitHelper.Execute(() =>
			{
				var fileName = FileConfigurationHelper.SaveToFile();
				if (!string.IsNullOrEmpty(fileName))
					FileName = fileName;
			});
		}

		public RelayCommand SaveAllCommand { get; private set; }
		void OnSaveAll()
		{
			ServiceFactory.Events.GetEvent<BeforeConfigurationSerializeEvent>().Publish(null);
			FileConfigurationHelper.SaveAllToFile();
		}

		public RelayCommand CreateNewCommand { get; private set; }
		void OnCreateNew()
		{
			ConfigManager.CreateNew();
		}

		public RelayCommand ValidateCommand { get; private set; }
		void OnValidate()
		{
			ServiceFactory.ValidationService.Validate();
		}

		public RelayCommand SetNewConfigCommand { get; private set; }
		void OnSetNewConfig()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите перезаписать текущую конфигурацию?") == MessageBoxResult.Yes)
			{
				if (!FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
				{
					MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
					return;
				}
				ConfigManager.SetNewConfig();
			}
		}
		public bool CanSetNewConfig()
		{
			return ServiceFactory.SaveService.HasChanges;
		}

		public RelayCommand SetPnanNameToZoneDescriptionsCommand { get; private set; }
		void OnSetPnanNameToZoneDescriptions()
		{
		}

		public RelayCommand MergeConfigurationCommand { get; private set; }
		void OnMergeConfiguration()
		{
			var mergeConfigurationHelper = new MergeConfigurationHelper();
			mergeConfigurationHelper.Merge();
		}

		private bool _isMainMenuVisible;
		public bool IsMainMenuVisible
		{
			get { return _isMainMenuVisible; }
			set
			{
				if (IsMainMenuVisible != value)
					RegistrySettingsHelper.SetBool("Administrato.Shell.IsMainMenuVisible", value);
				_isMainMenuVisible = value;
				OnPropertyChanged(() => IsMainMenuVisible);
			}
		}
		private bool _isMenuVisible;
		public bool IsMenuVisible
		{
			get { return _isMenuVisible; }
			set
			{
				if (IsMenuVisible != value)
					RegistrySettingsHelper.SetBool("Administrato.Shell.IsMenuVisible", value);
				_isMenuVisible = value;
				OnPropertyChanged(() => IsMenuVisible);
			}
		}
	}
}