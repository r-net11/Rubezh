using System;
using System.Collections.Generic;
using System.Linq;
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
        private string ViewName { get; set; }
        private Dictionary<string, Guid> Dictionary { get; set; }

		public LayoutPartCameraViewModel(Camera camera)
		{
			Camera = camera;
		    InitializeCommand();
		}

        public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
        {
            if (properties != null)
                Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
            InitializeCommand();
        }

        public LayoutPartCameraViewModel(string viewName, Dictionary<string, Guid> dictionary)
        {
            ViewName = viewName;
            Dictionary = dictionary;
            var cameraUID = dictionary.FirstOrDefault(x => x.Key == viewName).Value;
            if (cameraUID != Guid.Empty)
            {
                Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUID);
                CameraViewModel = new CameraViewModel(Camera);
            }
            InitializeCommand();
        }

	    public Camera Camera { get; set; }

        private CameraViewModel _cameraViewModel;
        public CameraViewModel CameraViewModel
        {
            get { return _cameraViewModel; }
            set
            {
                _cameraViewModel = value;
                OnPropertyChanged("CameraViewModel");
            }
        }

        void InitializeCommand()
        {
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            RemoveCommand = new RelayCommand(OnRemove, () => CameraViewModel != null);
        }

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var layoutPartPropertyCameraPageViewModel = new LayoutPartPropertyCameraPageViewModel(this);
		    if (DialogService.ShowModalWindow(layoutPartPropertyCameraPageViewModel))
		    {
		        Dictionary.Add(ViewName, Camera.UID);
		        CameraViewModel = new CameraViewModel(Camera);
		        CameraViewModel.StartVideo();
		    }
		}

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            Dictionary.Remove(ViewName);
            Camera = new Camera();
            CameraViewModel.StopVideo();
            CameraViewModel = null;
        }
	}
}