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
		public AdministratorShellViewModel()
			: base("Administrator")
		{
			Title = "Администратор ОПС FireSec-2";
			Height = 700;
			Width = 1000;
			MinWidth = 1000;
			MinHeight = 550;
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
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Проект", new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Новый", _menu.CreateNewCommand, "/Controls;component/Images/BNew.png", "Создать новую конфигурацию"),
				new RibbonMenuItemViewModel("Считать", _menu.LoadFromFileCommand, "/Controls;component/Images/BLoad.png", "Считать конфигурацию из файла"),
				new RibbonMenuItemViewModel("Сохранить", _menu.SaveAsCommand, "/Controls;component/Images/BSave.png", "Сохранить конфигурацию в файл"),
				new RibbonMenuItemViewModel("Проверить", _menu.ValidateCommand, "/Controls;component/Images/BCheck.png", "Проверить конфигурацию"),
				new RibbonMenuItemViewModel("Применить", _menu.SetNewConfigCommand, "/Controls;component/Images/BDownload.png", "Применить конфигурацию"),
			}, "/Controls;component/Images/BConfig.png", "Операции с конфигурацией") { Order = int.MinValue });
		}
	}
}