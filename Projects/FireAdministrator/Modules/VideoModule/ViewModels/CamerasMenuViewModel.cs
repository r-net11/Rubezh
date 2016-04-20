using Infrastructure.Common.Windows.Windows.ViewModels;
namespace VideoModule.ViewModels
{
	public class CamerasMenuViewModel : BaseViewModel
	{
		public CamerasMenuViewModel(CamerasViewModel context)
		{
			Context = context;
		}

		public CamerasViewModel Context { get; private set; }
	}
}