using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
	    private CameraViewModel _cameraViewModel;
		public CameraViewModel CameraViewModel {
		    get { return _cameraViewModel; }
		    set
		    {
		        _cameraViewModel = value;
		        OnPropertyChanged("CameraViewModel");
		    }}

		public LayoutPartCameraViewModel(Camera camera)
		{
			Camera = camera;
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

        private string ViewName { get; set; }
        private Dictionary<string, Guid> Dictionary { get; set; }
        public LayoutPartCameraViewModel(string viewName, Dictionary<string, Guid> dictionary)
        {
            ViewName = viewName;
            Dictionary = dictionary;
            Camera = new Camera();
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

	    public Camera Camera { get; set; }
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
			var layoutPartPropertyCameraPageViewModel = new LayoutPartPropertyCameraPageViewModel(this);
			DialogService.ShowModalWindow(layoutPartPropertyCameraPageViewModel);
            Dictionary.Add(ViewName, Camera.UID);
            CameraViewModel = new CameraViewModel(Camera);
            CameraViewModel.StartVideo();
		}
	}
}