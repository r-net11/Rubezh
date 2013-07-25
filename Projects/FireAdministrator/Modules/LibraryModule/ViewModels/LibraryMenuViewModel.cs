using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
	public class LibraryMenuViewModel : BaseViewModel
	{
		public LibraryMenuViewModel(LibraryViewModel context)
		{
			Context = context;
		}

		public LibraryViewModel Context { get; private set; }
	}
}