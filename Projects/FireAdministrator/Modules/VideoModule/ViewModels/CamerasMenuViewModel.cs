using Infrastructure.Common.Windows.ViewModels;
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