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

		public Infrastructure.Common.Windows.ShortcutService ShortcutService
		{
			get { return ApplicationLayoutService.ShortcutService; }
		}

		#endregion
	}
}