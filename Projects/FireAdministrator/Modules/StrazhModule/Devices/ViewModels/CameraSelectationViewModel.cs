using System.Collections.ObjectModel;
using System.Linq;
using Localization.Strazh.ViewModels;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class CameraSelectationViewModel : SaveCancelDialogViewModel
	{
		public CameraSelectationViewModel(Camera selectedCamera)
		{
		    Title = CommonViewModels.Camera_Selectation;

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