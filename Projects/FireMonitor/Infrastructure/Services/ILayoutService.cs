using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure
{
	public interface ILayoutService : Infrastructure.Common.Windows.Windows.ILayoutService
	{
		void AddAlarmGroups(BaseViewModel viewModel);
		void AddToolbarItem(BaseViewModel viewModel);
	}
}