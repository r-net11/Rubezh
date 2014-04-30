using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class PropertyViewModel : BaseViewModel
	{
		public string CellName { get; set; }
		public string PresentationCellName { get; set; }

		public PropertyViewModel(string cellName, string presentationCellName, Guid cameraUid)
		{
			CellName = cellName;
			PresentationCellName = presentationCellName;
			Cameras = new ObservableCollection<Camera>(FiresecManager.SystemConfiguration.AllCameras.FindAll(x => x.CameraType != CameraType.Dvr));
			Cameras.Insert(0, new Camera{UID = new Guid()});
			SelectedCamera = Cameras.FirstOrDefault(x => x.UID == cameraUid);
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
				OnPropertyChanged("SelectedCamera");
			}
		}
	}
}