using FireAdministrator.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

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