using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using FiresecClient;

namespace VideoModule.ViewModels
{
	public class LayoutPartPropertyCameraPageViewModel : SaveCancelDialogViewModel
	{
		public LayoutPartPropertyCameraPageViewModel()
		{
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
			return SelectedCamera != null;
		}

		protected override bool Save()
		{
			return true;
		}
	}
}