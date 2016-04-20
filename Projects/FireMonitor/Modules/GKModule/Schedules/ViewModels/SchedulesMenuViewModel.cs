using Infrastructure.Common.Windows.Windows.ViewModels;
namespace GKModule.ViewModels
{
	public class SchedulesMenuViewModel : BaseViewModel
	{
		public SchedulesMenuViewModel(SchedulesViewModel context)
		{
			Context = context;
		}

		public SchedulesViewModel Context { get; private set; }
	}
}