using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class CameraSelectionViewModel : SaveCancelDialogViewModel
	{
		public CameraSelectionViewModel(Camera camera)
		{
			Title = "Выбор видеоустройства";
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var cam in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(cam);
				Cameras.Add(cameraViewModel);
			}

			if (camera != null)
				SelectedCamera = Cameras.FirstOrDefault(x => x.Camera.UID == camera.UID);
			if (SelectedCamera == null)
				SelectedCamera = Cameras.FirstOrDefault();
		}

		public ObservableCollection<CameraViewModel> Cameras { get; private set; }

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				OnPropertyChanged(() => SelectedCamera);
			}
		}
	}
}