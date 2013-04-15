using Common;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows
{
	public interface ILayoutService
	{
		ShortcutService ShortcutService { get; }
		void Show(ViewPartViewModel model);
		void Close();
		void ShowToolbar(BaseViewModel model);
		void ShowFooter(BaseViewModel model);
		void ShowRightContent(RightContentViewModel model);
		bool IsRightPanelFocused { get; }
	}
}