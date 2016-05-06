using System;
using System.Linq;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class ReaderPropertiesViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }

		public ReaderPropertiesViewModel(SKDDevice device)
		{
			Device = device;
			Title = "Свойства считывателя " + Device.Name;
			SelectCameraCommand = new RelayCommand(OnSelectCamera);
			RemoveCameraCommand = new RelayCommand(OnRemoveCamera, CanRemove);

			Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == Device.CameraUID);
		}

		Camera _camera;
		public Camera Camera
		{
			get { return _camera; }
			set
			{
				_camera = value;
				OnPropertyChanged(() => Camera);
			}
		}

		public RelayCommand SelectCameraCommand { get; private set; }
		void OnSelectCamera()
		{
			var cameraSelectationViewModel = new CameraSelectationViewModel(Camera);
			if (DialogService.ShowModalWindow(cameraSelectationViewModel))
			{
				Camera = cameraSelectationViewModel.SelectedCamera;
			}
		}

		public RelayCommand RemoveCameraCommand { get; private set; }
		void OnRemoveCamera()
		{
			Camera = null;
		}
		bool CanRemove()
		{
			return Camera != null;
		}

		protected override bool Save()
		{
			Device.CameraUID = Camera == null ? Guid.Empty : Camera.UID;
			return true;
		}
	}
}