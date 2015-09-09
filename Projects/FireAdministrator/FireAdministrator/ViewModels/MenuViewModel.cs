﻿using System;
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
			CreateNewCommand = new RelayCommand(OnCreateNew);
			ValidateCommand = new RelayCommand(OnValidate);
			SetNewConfigCommand = new RelayCommand(OnSetNewConfig, CanSetNewConfig);
			ServiceFactory.SaveService.Changed += new Action(SaveService_Changed);

			// Подписываемся на событие "SetNewConfigurationEvent", которое могут рассылать модули с запросом к Администратору сохранить текущую конфигурацию
			ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Subscribe(OnSetNewConfiguration);
			
			IsMainMenuVisible = RegistrySettingsHelper.GetBool("Administrato.Shell.IsMainMenuVisible", true);
			IsMenuVisible = RegistrySettingsHelper.GetBool("Administrato.Shell.IsMenuVisible", true);
		}

		string logoSource;
		public string LogoSource
		{
			get { return logoSource; }
			set
			{
				logoSource = value;
				OnPropertyChanged(() => LogoSource);
			}
		}

		/// <summary>
		/// Сохраняет текущую конфигурацию
		/// </summary>
		/// <param name="e">Объект CancelEventArgs с результатом выполнения операции</param>
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
				FileConfigurationHelper.SaveToZipFile(FileName);
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
			var fileName = FileConfigurationHelper.SaveToFile();
			if (!string.IsNullOrEmpty(fileName))
				FileName = fileName;
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
			if (!MessageBoxService.ShowQuestion("Вы уверены, что хотите перезаписать текущую конфигурацию?")) return;

			if (!FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
				MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
				return;
			}
			ConfigManager.SetNewConfig();
		}
		public bool CanSetNewConfig()
		{
			return ServiceFactory.SaveService.HasChanges;
		}

		bool _isMainMenuVisible;
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
		bool _isMenuVisible;
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