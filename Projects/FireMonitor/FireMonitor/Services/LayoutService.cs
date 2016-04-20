using FireMonitor.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace FireMonitor
{
	public class LayoutService : ILayoutService
	{
		private Infrastructure.Common.Windows.Windows.ILayoutService ApplicationLayoutService
		{
			get { return Infrastructure.Common.Windows.Windows.ApplicationService.Layout; }
		}
		private ToolbarViewModel _toolbarViewModel;
		internal void SetToolbarViewModel(ToolbarViewModel toolbarViewModel)
		{
			_toolbarViewModel = toolbarViewModel;
		}

		#region ILayoutService Members

		public void AddAlarmGroups(BaseViewModel viewModel)
		{
			_toolbarViewModel.AlarmGroups = viewModel;
		}
		public void AddToolbarItem(BaseViewModel viewModel)
		{
			_toolbarViewModel.ToolbarItems.Add(viewModel);
		}

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

		public Infrastructure.Common.Windows.Windows.ShortcutService ShortcutService
		{
			get { return ApplicationLayoutService.ShortcutService; }
		}

		public void ShowRightContent(RightContentViewModel model)
		{
			ApplicationLayoutService.ShowRightContent(model);
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