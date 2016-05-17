using RubezhMonitor.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace RubezhMonitor
{
	public class LayoutService : ILayoutService
	{
		private Infrastructure.Common.Windows.ILayoutService ApplicationLayoutService
		{
			get { return Infrastructure.Common.Windows.ApplicationService.Layout; }
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

		public Infrastructure.Common.Windows.ShortcutService ShortcutService
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