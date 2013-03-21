using System.Collections.Generic;
using System.Windows.Controls;
using Common;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows
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

		public void Show(ViewPartViewModel viewModel)
		{
			ViewPartViewModel exist = null;
			foreach (ViewPartViewModel item in ShellViewModel.ContentItems)
				if (item.Key == viewModel.Key)
					exist = item;
				else
				{
					if (item.IsActive)
						item.IsRightPanelVisible = ShellViewModel.RightPanelVisible;
					item.Hide();
				}
			if (exist == null)
			{
				ShellViewModel.ContentItems.Add(viewModel);
				exist = viewModel;
			}
			ShellViewModel.IsRightPanelEnabled = exist.IsRightPanelEnabled;
			ShellViewModel.RightPanelVisible = exist.IsRightPanelVisible;
			exist.Show();
		}
		public void Close()
		{
			foreach (ViewPartViewModel item in ShellViewModel.ContentItems)
				item.Hide();
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