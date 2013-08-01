using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Ribbon;
using System.Collections.ObjectModel;

namespace FireAdministrator.ViewModels
{
	public class AdministratorShellViewModel : ShellViewModel
	{
		private MenuViewModel _menu;
		private RibbonMenuItemViewModel _showToolbar;
		private RibbonMenuItemViewModel _showMenu;

		public AdministratorShellViewModel()
			: base("Administrator")
		{
			Title = "Администратор ОПС FireSec-2";
			Height = 700;
			Width = 1000;
			MinWidth = 1000;
			MinHeight = 550;
			ShowToolbarCommand = new RelayCommand(OnShowToolbar, CanShowMenu);
			ShowMenuCommand = new RelayCommand(OnShowMenu, CanShowMenu);
			_menu = new MenuViewModel();
			Toolbar = _menu;
			RibbonContent = new RibbonMenuViewModel();
			AddRibbonItem();
			AllowLogoIcon = false;
			RibbonVisible = true;
		}

		public override bool OnClosing(bool isCanceled)
		{
			AlarmPlayerHelper.Dispose();
			if (ServiceFactory.SaveService.HasChanges)
			{
				var result = MessageBoxService.ShowQuestion("Применить изменения в настройках?");
				switch (result)
				{
					case MessageBoxResult.Yes:
						return !((MenuViewModel)Toolbar).SetNewConfig();
					case MessageBoxResult.No:
						return false;
					case MessageBoxResult.Cancel:
						return true;
				}
			}
			return base.OnClosing(isCanceled);
		}
		public override void OnClosed()
		{
			FiresecManager.Disconnect();
			base.OnClosed();
		}

		private void AddRibbonItem()
		{
			_showToolbar = new RibbonMenuItemViewModel("", ShowToolbarCommand, "/Controls;component/Images/BToolbar.png");
			_showMenu = new RibbonMenuItemViewModel("", ShowMenuCommand, "/Controls;component/Images/BMenu.png");
			UpdateToolbarTitle();
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Проект", new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Новый", _menu.CreateNewCommand, "/Controls;component/Images/BNew.png", "Создать новую конфигурацию"),
				new RibbonMenuItemViewModel("Открыть", _menu.LoadFromFileCommand, "/Controls;component/Images/BLoad.png", "Открыть конфигурацию из файла"),
				new RibbonMenuItemViewModel("Проверить", _menu.ValidateCommand, "/Controls;component/Images/BCheck.png", "Проверить конфигурацию"),
				new RibbonMenuItemViewModel("Применить", _menu.SetNewConfigCommand, "/Controls;component/Images/BDownload.png", "Применить конфигурацию"),
				new RibbonMenuItemViewModel("Сохранить", _menu.SaveCommand, "/Controls;component/Images/BSave.png", "Сохранить конфигурацию в файл"),
				new RibbonMenuItemViewModel("Сохранить как", _menu.SaveAsCommand, "/Controls;component/Images/BSaveAs.png", "Сохранить как"),
				new RibbonMenuItemViewModel("Сохранить все", _menu.SaveAllCommand, "/Controls;component/Images/BSaveAll.png", "Сохранить все"),
				new RibbonMenuItemViewModel("Слияние конфигураций", _menu.MergeConfigurationCommand, "/Controls;component/Images/BAllParameters.png", "Слияние конфигураций"),
				new RibbonMenuItemViewModel("Вид", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					_showMenu,
					_showToolbar,
				}, "/Controls;component/Images/BView.png") { Order = 1000 }, 
			}, "/Controls;component/Images/BConfig.png", "Операции с конфигурацией") { Order = int.MinValue });
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Выход", ApplicationCloseCommand, "/Controls;component/Images/BExit.png") { Order = int.MaxValue });
		}

		public RelayCommand ShowToolbarCommand { get; private set; }
		private void OnShowToolbar()
		{
			_menu.IsMenuVisible = !_menu.IsMenuVisible;
			UpdateToolbarTitle();
		}
		public RelayCommand ShowMenuCommand { get; private set; }
		private void OnShowMenu()
		{
			_menu.IsMainMenuVisible = !_menu.IsMainMenuVisible;
			UpdateToolbarTitle();
		}
		private bool CanShowMenu()
		{
			return ToolbarVisible;
		}

		private void UpdateToolbarTitle()
		{
			_showToolbar.Text = _menu.IsMenuVisible ? "Скрыть панель инструментов" : "Показать панель инструментов";
			_showMenu.Text = _menu.IsMainMenuVisible ? "Скрыть панель меню" : "Показать панель меню";
		}

	}
}