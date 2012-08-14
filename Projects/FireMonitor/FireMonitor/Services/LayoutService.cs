using FireMonitor.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor
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