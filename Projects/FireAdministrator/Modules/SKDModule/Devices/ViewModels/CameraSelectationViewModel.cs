using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CameraSelectationViewModel : SaveCancelDialogViewModel
	{
		public CameraSelectationViewModel(Camera selectedCamera)
		{
			Title = "Выбор камеры";

			Cameras = new ObservableCollection<Camera>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				Cameras.Add(camera);
			}
			if (selectedCamera != null)
				SelectedCamera = Cameras.FirstOrDefault(x => x.UID == selectedCamera.UID);
		}

		public ObservableCollection<Camera> Cameras { get; private set; }

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

		protected override bool Save()
		{
			return true;
		}
	}
}