using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutsMenuViewModel : BaseViewModel
	{
		public MonitorLayoutsMenuViewModel(MonitorLayoutsViewModel context)
		{
			Context = context;
		}

		public MonitorLayoutsViewModel Context { get; private set; }
	}
}