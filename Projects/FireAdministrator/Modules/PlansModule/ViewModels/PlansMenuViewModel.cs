
using Infrastructure.Common.Windows.ViewModels;
namespace PlansModule.ViewModels
{
	public class PlansMenuViewModel : BaseViewModel
	{
		public PlansMenuViewModel(PlansViewModel context)
		{
			Context = context;
		}

		public PlansViewModel Context { get; private set; }
	}
}
