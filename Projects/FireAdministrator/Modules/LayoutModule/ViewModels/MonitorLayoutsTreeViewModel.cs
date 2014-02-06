using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutsTreeViewModel : BaseViewModel
	{
		public MonitorLayoutsTreeViewModel(MonitorLayoutsViewModel layouts)
		{
			Layouts = layouts;
		}

		public MonitorLayoutsViewModel Layouts { get; private set; }
	}
}