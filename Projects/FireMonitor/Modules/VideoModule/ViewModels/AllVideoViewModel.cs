using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class AllVideoViewModel : ViewPartViewModel
	{
		public CamerasViewModel CamerasViewModel { get; private set; }
		public LayoutMultiCameraViewModel LayoutMultiCameraViewModel { get; private set; }

		public AllVideoViewModel()
		{
			CamerasViewModel = new CamerasViewModel();
			LayoutMultiCameraViewModel = new LayoutMultiCameraViewModel();
		}

		public void Initialize()
		{
			CamerasViewModel.Initialize();
		}
	}
}