using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public CameraViewModel CameraViewModel { get; set; }

        public LayoutPartCameraViewModel(Camera camera)
        {
            Camera = camera;
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public LayoutPartCameraViewModel()
        {
            //CameraViewModel = new CameraViewModel(FiresecManager.SystemConfiguration.Cameras.FirstOrDefault());
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        Camera _camera;
        public Camera Camera
        {
            get { return _camera; }
            set
            {
                _camera = value;
                CameraViewModel = new CameraViewModel(value);
                CameraViewModel.StartVideo();
            }
        }

        public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
		{
			if (properties != null)
			{
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
			}
		}

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault();
            //var layoutPartPropertyCameraPageViewModel = new LayoutPartPropertyCameraPageViewModel(this);
            //DialogService.ShowModalWindow(layoutPartPropertyCameraPageViewModel);            
        }
	}
}