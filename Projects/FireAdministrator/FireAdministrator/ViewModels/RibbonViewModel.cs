using System;
using System.ComponentModel;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Ribbon;
using System.Collections.ObjectModel;

namespace FireAdministrator.ViewModels
{
	public class RibbonViewModel : RibbonMenuViewModel
	{
		private MenuViewModel _menu;

		public RibbonViewModel(MenuViewModel menuViewModel)
		{
			_menu = menuViewModel;
			Items = new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Конфигурация", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Новый", _menu.CreateNewCommand, "/Controls;component/Images/BNew.png", "Создать новую конфигурацию"),
					new RibbonMenuItemViewModel("Считать", _menu.LoadFromFileCommand, "/Controls;component/Images/BLoad.png", "Считать конфигурацию из файла"),
					new RibbonMenuItemViewModel("Сохранить", _menu.SaveAsCommand, "/Controls;component/Images/BSave.png", "Сохранить конфигурацию в файл"),
					new RibbonMenuItemViewModel("Проверить", _menu.ValidateCommand, "/Controls;component/Images/BCheck.png", "Проверить конфигурацию"),
					new RibbonMenuItemViewModel("Применить", _menu.SetNewConfigCommand, "/Controls;component/Images/BDownload.png", "Применить конфигурацию"),
				}, "/Controls;component/Images/BConfig.png", "Операции с конфигурацией"),
			};
		}

	}
}