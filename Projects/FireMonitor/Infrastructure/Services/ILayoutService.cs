using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure
{
	public interface ILayoutService : Infrastructure.Common.Windows.ILayoutService
	{
		void AddAlarmGroups(BaseViewModel viewModel);
		void AddToolbarItem(BaseViewModel viewModel);
	}
}