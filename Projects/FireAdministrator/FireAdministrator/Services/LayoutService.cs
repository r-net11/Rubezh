using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FireAdministrator.Views;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FireAdministrator.ViewModels;

namespace FireAdministrator
{
	public class LayoutService : ILayoutService
	{
		private Infrastructure.Common.Windows.ILayoutService ApplicationLayoutService
		{
			get { return Infrastructure.Common.Windows.ApplicationService.Layout; }
		}
		private MenuViewModel _menuViewModel;

		internal void SetMenuViewModel(MenuViewModel menuViewModel)
		{
			_menuViewModel = menuViewModel;
		}

		#region ILayoutService Members

		public void ShowMenu(BaseViewModel viewModel)
		{
			_menuViewModel.ExtendedMenu = viewModel;
		}

		#endregion

		#region ILayoutService Members

		public void Show(ViewPartViewModel model)
		{
			ApplicationLayoutService.Show(model);
		}

		public void Close()
		{
			ApplicationLayoutService.Close();
		}

		public void ShowToolbar(BaseViewModel model)
		{
			ApplicationLayoutService.ShowToolbar(model);
		}

		public void ShowHeader(BaseViewModel model)
		{
			ApplicationLayoutService.ShowHeader(model);
		}

		public void ShowFooter(BaseViewModel model)
		{
			ApplicationLayoutService.ShowFooter(model);
		}

		#endregion
	}
}