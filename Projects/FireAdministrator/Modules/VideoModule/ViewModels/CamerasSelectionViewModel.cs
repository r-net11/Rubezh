using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Models;

namespace VideoModule.ViewModels
{
	[SaveSizeAttribute]
	public class CamerasSelectionViewModel : SaveCancelDialogViewModel
	{
		public List<Camera> Cameras { get; private set; }

		public CamerasSelectionViewModel(List<Camera> cameras)
		{
			Title = "Выбор камер";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			Cameras = cameras;
			TargetCameras = new ObservableCollection<Camera>();
			SourceCameras = new ObservableCollection<Camera>();

			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
			{
				if (Cameras.Contains(camera))
					TargetCameras.Add(camera);
				else
					SourceCameras.Add(camera);
			}

			SelectedTargetCamera = TargetCameras.FirstOrDefault();
			SelectedSourceCamera = SourceCameras.FirstOrDefault();
		}

		public ObservableCollection<Camera> SourceCameras { get; private set; }

		Camera _selectedSourceCamera;
		public Camera SelectedSourceCamera
		{
			get { return _selectedSourceCamera; }
			set
			{
				_selectedSourceCamera = value;
				OnPropertyChanged(() => SelectedSourceCamera);
			}
		}

		public ObservableCollection<Camera> TargetCameras { get; private set; }

		Camera _selectedTargetCamera;
		public Camera SelectedTargetCamera
		{
			get { return _selectedTargetCamera; }
			set
			{
				_selectedTargetCamera = value;
				OnPropertyChanged(() => SelectedTargetCamera);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceCameras;
		void OnAdd(object parameter)
		{
			var index = SourceCameras.IndexOf(SelectedSourceCamera);

			SelectedSourceCameras = (IList)parameter;
			var cameraViewModels = new List<Camera>();
			foreach (var selectedCamera in SelectedSourceCameras)
			{
				var cameraViewModel = selectedCamera as Camera;
				if (cameraViewModel != null)
					cameraViewModels.Add(cameraViewModel);
			}
			foreach (var cameraViewModel in cameraViewModels)
			{
				TargetCameras.Add(cameraViewModel);
				SourceCameras.Remove(cameraViewModel);
			}
			SelectedTargetCamera = TargetCameras.LastOrDefault();
			OnPropertyChanged("SourceCameras");

			index = Math.Min(index, SourceCameras.Count - 1);
			if (index > -1)
				SelectedSourceCamera = SourceCameras[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetCameras;
		void OnRemove(object parameter)
		{
			var index = TargetCameras.IndexOf(SelectedTargetCamera);

			SelectedTargetCameras = (IList)parameter;
			var cameraViewModels = new List<Camera>();
			foreach (var selectedCamera in SelectedTargetCameras)
			{
				var cameraViewModel = selectedCamera as Camera;
				if (cameraViewModel != null)
					cameraViewModels.Add(cameraViewModel);
			}
			foreach (var cameraViewModel in cameraViewModels)
			{
				SourceCameras.Add(cameraViewModel);
				TargetCameras.Remove(cameraViewModel);
			}
			SelectedSourceCamera = SourceCameras.LastOrDefault();
			OnPropertyChanged(() => TargetCameras);

			index = Math.Min(index, TargetCameras.Count - 1);
			if (index > -1)
				SelectedTargetCamera = TargetCameras[index];
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var cameraViewModel in SourceCameras)
			{
				TargetCameras.Add(cameraViewModel);
			}
			SourceCameras.Clear();
			SelectedTargetCamera = TargetCameras.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var cameraViewModel in TargetCameras)
			{
				SourceCameras.Add(cameraViewModel);
			}
			TargetCameras.Clear();
			SelectedSourceCamera = SourceCameras.FirstOrDefault();
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceCamera != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetCamera != null;
		}

		public bool CanAddAll()
		{
			return (SourceCameras.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetCameras.Count > 0);
		}

		protected override bool Save()
		{
			Cameras = new List<Camera>(TargetCameras);
			return base.Save();
		}
	}
}