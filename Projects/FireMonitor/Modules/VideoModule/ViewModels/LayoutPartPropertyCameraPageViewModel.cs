using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;

namespace VideoModule.ViewModels
{
    public class LayoutPartPropertyCameraPageViewModel : SaveCancelDialogViewModel
    {
        LayoutPartCameraViewModel _layoutPartCameraViewModel { get; set; }
        public LayoutPartPropertyCameraPageViewModel(LayoutPartCameraViewModel layoutPartCameraViewModel)
        {
            _layoutPartCameraViewModel = layoutPartCameraViewModel;
            Cameras = new ObservableCollection<Camera>(FiresecManager.SystemConfiguration.Cameras);
        }

        private ObservableCollection<Camera> _cameras;
        public ObservableCollection<Camera> Cameras
        {
            get { return _cameras; }
            set
            {
                _cameras = value;
                OnPropertyChanged(() => Cameras);
            }
        }

        private Camera _selectedCamera;
        public Camera SelectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                _selectedCamera = value;
                OnPropertyChanged(() => SelectedCamera);
            }
        }
        
        protected override bool CanSave()
        {
            return true;
        }

        protected override bool Save()
        {
            if (SelectedCamera != null)
            {
                _layoutPartCameraViewModel.Camera = SelectedCamera;
                _layoutPartCameraViewModel.CameraViewModel.StartVideo();
                return true;
            }
            return false;
        }
    }
}
