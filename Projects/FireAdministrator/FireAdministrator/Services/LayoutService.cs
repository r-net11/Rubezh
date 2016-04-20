using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace FireAdministrator
{
	public class LayoutService : Infrastructure.ILayoutService
	{
		private Infrastructure.Common.Windows.Windows.ILayoutService ApplicationLayoutService
		{
			get { return Infrastructure.Common.Windows.Windows.ApplicationService.Layout; }
		}

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
		public void ShowFooter(BaseViewModel model)
		{
			ApplicationLayoutService.ShowFooter(model);
		}
		public void ShowRightContent(RightContentViewModel model)
		{
			ApplicationLayoutService.ShowRightContent(model);
		}

		public ShortcutService ShortcutService
		{
			get { return ApplicationLayoutService.ShortcutService; }
		}

		public bool IsRightPanelFocused
		{
			get { return ApplicationLayoutService.IsRightPanelFocused; }
		}

		public void SetRightPanelVisible(bool isVisible)
		{
			ApplicationLayoutService.SetRightPanelVisible(isVisible);
		}

		public void SetLeftPanelVisible(bool isVisible)
		{
			ApplicationLayoutService.SetLeftPanelVisible(isVisible);
		}

		#endregion
	}
}