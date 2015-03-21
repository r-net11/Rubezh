using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorsMenuViewModel : BaseViewModel
	{
		public DoorsMenuViewModel(DoorsViewModel doorsViewModel)
		{
			Context = doorsViewModel;
		}

		public DoorsViewModel Context { get; private set; }
	}
}