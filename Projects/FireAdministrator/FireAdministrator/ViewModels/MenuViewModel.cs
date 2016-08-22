using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Localization.FireAdministrator.ViewModels;
using StrazhAPI.Models;
using System.ComponentModel;

namespace FireAdministrator.ViewModels
{
	public class MenuViewModel : BaseViewModel
	{
		#region Fields
		private string _fileName;
		private string _logoSource;
		private bool _hasChanges = true;
		private bool _isMainMenuVisible;
		bool _isMenuVisible;
		private BaseViewModel _extendedMenu;
		#endregion

		public MenuViewModel()
		{
			LoadFromFileCommand = new RelayCommand(OnLoadFromFile);
			SaveCommand = new RelayCommand(OnSave, () => _hasChanges);
			SaveAsCommand = new RelayCommand(OnSaveAs);
			CreateNewCommand = new RelayCommand(OnCreateNew);
			ValidateCommand = new RelayCommand(OnValidate);
			SetNewConfigCommand = new RelayCommand(OnSetNewConfig, CanSetNewConfig);
			ServiceFactory.SaveService.Changed += SaveService_Changed;

			// Подписываемся на событие "SetNewConfigurationEvent", которое могут рассылать модули с запросом к Администратору сохранить текущую конфигурацию
			ServiceFactoryBase.Events.GetEvent<SetNewConfigurationEvent>().Subscribe(OnSetNewConfiguration);

			IsMainMenuVisible = RegistrySettingsHelper.GetBool("Administrato.Shell.IsMainMenuVisible", true);
			IsMenuVisible = RegistrySettingsHelper.GetBool("Administrato.Shell.IsMenuVisible", true);
		}

		#region Properties
		public string LogoSource
		{
			get { return _logoSource; }
			set
			{
				_logoSource = value;
				OnPropertyChanged(() => LogoSource);
			}
		}

		public BaseViewModel ExtendedMenu
		{
			get { return _extendedMenu; }
			set
			{
				_extendedMenu = value;
				OnPropertyChanged(() => ExtendedMenu);
			}
		}

		public bool CanShowMainMenu
		{
			get { return false; }
		}

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
		#endregion

		/// <summary>
		/// Сохраняет текущую конфигурацию
		/// </summary>
		/// <param name="e">Объект CancelEventArgs с результатом выполнения операции</param>
		private static void OnSetNewConfiguration(CancelEventArgs e)
		{
			if (!FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
                MessageBoxService.Show(CommonViewModels.NoPermissionSaveConfig);
				e.Cancel = true;
				return;
			}
			e.Cancel = !ConfigManager.SetNewConfig();
		}

		public bool SetNewConfig()
		{
			if (FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig)) return ConfigManager.SetNewConfig();

            MessageBoxService.Show(CommonViewModels.NoPermissionSaveConfig);
			return false;
		}

		private void SaveService_Changed()
		{
			_hasChanges = ServiceFactory.SaveService.HasChanges;
		}

		private void OnLoadFromFile()
		{
			var fileName = FileConfigurationHelper.LoadFromFile();
			if (!string.IsNullOrEmpty(fileName))
				_fileName = fileName;
		}

		private void OnSave()
		{
			ServiceFactoryBase.Events.GetEvent<BeforeConfigurationSerializeEvent>().Publish(null);

			if (string.IsNullOrEmpty(_fileName))
				OnSaveAs();
			else
				FileConfigurationHelper.SaveToZipFile(_fileName);
		}

		private void OnSaveAs()
		{
			ServiceFactoryBase.Events.GetEvent<BeforeConfigurationSerializeEvent>().Publish(null);
			var fileName = FileConfigurationHelper.SaveToFile();
			if (!string.IsNullOrEmpty(fileName))
				_fileName = fileName;
		}

		private static void OnCreateNew()
		{
			ConfigManager.CreateNew();
		}

		private static void OnValidate()
		{
			ServiceFactory.ValidationService.Validate();
		}

		private static void OnSetNewConfig()
		{
            if (!MessageBoxService.ShowQuestion(CommonViewModels.OverwriteConfigQuestion)) return;

			if (!FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
                MessageBoxService.Show(CommonViewModels.NoPermissionSaveConfig);
				return;
			}
			ConfigManager.SetNewConfig();
		}

		public bool CanSetNewConfig()
		{
			return ServiceFactory.SaveService.HasChanges;
		}

		public RelayCommand LoadFromFileCommand { get; private set; }
		public RelayCommand SetNewConfigCommand { get; private set; }
		public RelayCommand ValidateCommand { get; private set; }
		public RelayCommand CreateNewCommand { get; private set; }
		public RelayCommand SaveAsCommand { get; private set; }
		public RelayCommand SaveCommand { get; private set; }
	}
}