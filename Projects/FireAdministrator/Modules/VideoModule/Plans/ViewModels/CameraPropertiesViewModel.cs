﻿using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VideoModule.ViewModels;

namespace VideoModule.Plans.ViewModels
{
	public class CameraPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementCamera _elementCamera;
		CamerasViewModel _camerasViewModel;
		List<CameraViewModel> AllCameras { get; set; }

		public CameraPropertiesViewModel(CamerasViewModel camerasViewModel, ElementCamera elementCamera)
		{
			Title = "Свойства фигуры: Камера";
			_elementCamera = elementCamera;
			_camerasViewModel = camerasViewModel;

			Cameras = camerasViewModel.Cameras;
			AllCameras = camerasViewModel.AllCameras;
			SelectedCamera = AllCameras.FirstOrDefault(item => item.IsCamera && item.Camera.UID == elementCamera.CameraUID);
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

		protected override bool Save()
		{
			Guid cameraUID = _elementCamera.CameraUID;
			PlanExtension.Instance.SetItem(_elementCamera, SelectedCamera.Camera);

			if (cameraUID != _elementCamera.CameraUID)
				Update(cameraUID);
			_camerasViewModel.SelectedCamera = Update(_elementCamera.CameraUID);
			return base.Save();
		}
		CameraViewModel Update(Guid cameraUID)
		{
			var camera = AllCameras.FirstOrDefault(x => x.IsCamera && x.Camera.UID == cameraUID);
			if (camera != null)
				camera.Update();
			return camera;
		}

		protected override bool CanSave()
		{
			if (SelectedCamera == null)
				return false;
			if (!SelectedCamera.IsCamera)
				return false;
			if (SelectedCamera.IsOnPlan && !SelectedCamera.Camera.AllowMultipleVizualization && SelectedCamera.Camera.UID != _elementCamera.CameraUID)
				return false;
			return true;
		}
	}
}