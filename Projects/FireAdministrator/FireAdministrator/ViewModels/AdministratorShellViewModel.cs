using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Localization.FireAdministrator.Common;
using Localization.FireAdministrator.ViewModels;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace FireAdministrator.ViewModels
{
	public class AdministratorShellViewModel : ShellViewModel
	{
		MenuViewModel _menu;
		RibbonMenuItemViewModel _showToolbar;
		RibbonMenuItemViewModel _showMenu;

		public AdministratorShellViewModel()
			: base(ClientType.Administrator)
		{
			Title = CommonResources.Administator;
			Height = 700;
			Width = 1000;
			MinWidth = 1000;
			MinHeight = 550;
			ShowToolbarCommand = new RelayCommand(OnShowToolbar, CanShowMenu);
			ShowMenuCommand = new RelayCommand(OnShowMenu, CanShowMenu);
			_menu = new MenuViewModel();
			_menu.LogoSource = "Logo";
			Toolbar = _menu;
			RibbonContent = new RibbonMenuViewModel();
			AddRibbonItem();
			AllowLogoIcon = false;
			RibbonVisible = true;
		}

		public override void Run()
		{
			base.Run();
			ServiceFactory.Layout.ShortcutService.Shortcuts.Add(new KeyGesture(Key.S, ModifierKeys.Control), _menu.SaveCommand);			
		}
		public override bool OnClosing(bool isCanceled)
		{
			try
			{
				AlarmPlayerHelper.Dispose();
				if (ServiceFactory.SaveService.HasChanges)
				{
                    var result = MessageBoxService.ShowQuestionExtended(CommonViewModels.AcceptConfigQuestion);
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
				FiresecManager.Disconnect();
				return base.OnClosing(isCanceled);
			}
			finally
			{

			}
		}

		public override void OnClosed()
		{
			//FiresecManager.Disconnect();
			base.OnClosed();
		}

		void AddRibbonItem()
		{
			_showToolbar = new RibbonMenuItemViewModel("", ShowToolbarCommand, "BToolbar");
			_showMenu = new RibbonMenuItemViewModel("", ShowMenuCommand, "BMenu");
			UpdateToolbarTitle();
			RibbonContent.Items.Add(new RibbonMenuItemViewModel(CommonViewModels.Project, new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel(CommonViewModels.New, _menu.CreateNewCommand, "BNew", CommonResources.CreateNewConfig),
				new RibbonMenuItemViewModel(CommonViewModels.Open, _menu.LoadFromFileCommand, "BLoad",CommonResources.FileConfigOpen),
				new RibbonMenuItemViewModel(CommonViewModels.Check, _menu.ValidateCommand, "BCheck", CommonResources.CheckConfig),
				new RibbonMenuItemViewModel(CommonViewModels.Accept, _menu.SetNewConfigCommand, "BDownload", CommonResources.AcceptConfig),
				new RibbonMenuItemViewModel(CommonViewModels.Save, _menu.SaveCommand, "BSave", CommonResources.FileConfigSave),
				new RibbonMenuItemViewModel(CommonResources.SaveAs, _menu.SaveAsCommand, "BSaveAs", CommonResources.SaveAs),
				new RibbonMenuItemViewModel(CommonViewModels.View, new ObservableCollection<RibbonMenuItemViewModel>()
				{
					_showMenu,
					_showToolbar,
				}, "BView") { Order = 1000 }, 
			}, "BConfig", CommonViewModels.ConfigOperations) { Order = int.MinValue });
			RibbonContent.Items.Add(new RibbonMenuItemViewModel(CommonViewModels.Exit, ApplicationCloseCommand, "BExit") { Order = int.MaxValue });
		}

		public RelayCommand ShowToolbarCommand { get; private set; }
		void OnShowToolbar()
		{
			_menu.IsMenuVisible = !_menu.IsMenuVisible;
			UpdateToolbarTitle();
		}
		public RelayCommand ShowMenuCommand { get; private set; }
		void OnShowMenu()
		{
			_menu.IsMainMenuVisible = !_menu.IsMainMenuVisible;
			UpdateToolbarTitle();
		}
		bool CanShowMenu()
		{
			return ToolbarVisible;
		}

		void UpdateToolbarTitle()
		{
            _showToolbar.Text = _menu.IsMenuVisible ? CommonViewModels.HideInstrumentPanel : CommonViewModels.ShowInstrumentPanel;
            _showMenu.Text = _menu.IsMainMenuVisible ? CommonViewModels.HideMenuPanel : CommonViewModels.ShowMenuPanel;
		}
	}
}