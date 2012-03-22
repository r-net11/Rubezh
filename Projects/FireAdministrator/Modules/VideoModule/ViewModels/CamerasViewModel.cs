using Infrastructure.Common;
using Infrastructure;
using VideoModule.Views;
using System.Collections.ObjectModel;
using FiresecClient;
using Controls.MessageBox;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace VideoModule.ViewModels
{
    public class CamerasViewModel : RegionViewModel, IEditingViewModel
    {
        public CamerasViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete);
            EditCommand = new RelayCommand(OnEdit);

            Cameras = new ObservableCollection<CameraViewModel>();

            if (FiresecManager.SystemConfiguration.Cameras == null)
                FiresecManager.SystemConfiguration.Cameras = new List<Camera>();

            foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
            {
                var cameraViewModel = new CameraViewModel(camera);
                Cameras.Add(cameraViewModel);
            }

            if (Cameras.Count > 0)
                SelectedCamera = Cameras[0];
        }

        public ObservableCollection<CameraViewModel> Cameras { get; private set; }

        CameraViewModel _selectedCamera;
        public CameraViewModel SelectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                _selectedCamera = value;
                OnPropertyChanged("SelectedZone");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var cameraDetailsViewModel = new CameraDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(cameraDetailsViewModel))
            {
                FiresecManager.SystemConfiguration.Cameras.Add(cameraDetailsViewModel.Camera);
                Cameras.Add(new CameraViewModel(cameraDetailsViewModel.Camera));
                ServiceFactory.SaveService.CamerasChanged = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.SystemConfiguration.Cameras.Remove(SelectedCamera.Camera);
            Cameras.Remove(SelectedCamera);
            ServiceFactory.SaveService.CamerasChanged = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var cameraDetailsViewModel = new CameraDetailsViewModel(SelectedCamera.Camera);
            if (ServiceFactory.UserDialogs.ShowModalWindow(cameraDetailsViewModel))
            {
                SelectedCamera.Camera = cameraDetailsViewModel.Camera;
                SelectedCamera.Update();
                ServiceFactory.SaveService.CamerasChanged = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new CamerasMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}