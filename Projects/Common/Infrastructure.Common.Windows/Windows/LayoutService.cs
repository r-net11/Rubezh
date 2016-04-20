using System.Collections.Generic;
using System.Windows.Controls;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Windows
{
	internal class LayoutService : ILayoutService
	{
		public ShortcutService ShortcutService { get; private set; }
		public ShellViewModel ShellViewModel { get; private set; }
		private Dictionary<string, ContentPresenter> _cache;

		internal LayoutService(ShellViewModel shellViewModel)
		{
			ShellViewModel = shellViewModel;
			ShortcutService = new ShortcutService();
			_cache = new Dictionary<string, ContentPresenter>();
		}

		public bool IsRightPanelFocused
		{
			get { return ShellViewModel == null ? false : ShellViewModel.IsRightPanelFocused; }
		}
		public void SetRightPanelVisible(bool isVisible)
		{
			if (ShellViewModel != null)
				ShellViewModel.RightPanelVisible = isVisible;
		}
		public void SetLeftPanelVisible(bool isVisible)
		{
			if (ShellViewModel != null)
				ShellViewModel.LeftPanelVisible = isVisible;
		}


		public void Show(ViewPartViewModel viewModel)
		{
			if (ServiceFactoryBase.DragDropService != null)
				ServiceFactoryBase.DragDropService.StopDragSimulate();
			ViewPartViewModel exist = null;
			foreach (ViewPartViewModel item in ShellViewModel.ContentItems)
			{
				if (item.IsActive)
					item.IsRightPanelVisible = ShellViewModel.RightPanelVisible;
				if (item.Key == viewModel.Key)
					exist = item;
				else
					item.Hide();
			}
			if (exist == null)
			{
				ShellViewModel.ContentItems.Add(viewModel);
				exist = viewModel;
			}
			ShellViewModel.IsRightPanelEnabled = exist.IsRightPanelEnabled;
			ShellViewModel.LeftPanelVisible = true;
			ShellViewModel.RightPanelVisible = exist.IsRightPanelVisible;
			exist.Show();
		}
		public void Close()
		{
			foreach (ViewPartViewModel item in ShellViewModel.ContentItems)
			{
				if (item.IsActive)
					item.IsRightPanelVisible = ShellViewModel.RightPanelVisible;
				item.Hide();
			}
		}

		public void ShowToolbar(BaseViewModel model)
		{
			ShellViewModel.Toolbar = model;
		}
		public void ShowFooter(BaseViewModel model)
		{
			ShellViewModel.ContentFotter = model;
		}
		public void ShowRightContent(RightContentViewModel model)
		{
			ShellViewModel.RightContent = model;
			model.Content.Show();
		}
	}
}