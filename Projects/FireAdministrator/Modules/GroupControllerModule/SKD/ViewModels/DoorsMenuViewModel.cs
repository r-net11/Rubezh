using Infrastructure.Common.Windows.Windows.ViewModels;
namespace GKModule.ViewModels
{
	public class DoorsMenuViewModel : BaseViewModel
	{
		public DoorsMenuViewModel(DoorsViewModel context)
		{
			Context = context;
		}

		public DoorsViewModel Context { get; private set; }
	}
}