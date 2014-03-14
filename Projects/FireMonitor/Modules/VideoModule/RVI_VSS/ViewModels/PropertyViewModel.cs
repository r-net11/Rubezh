using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.RVI_VSS.ViewModels
{
	public class PropertyViewModel: BaseViewModel
	{
		public string CellName { get; set; }
		public PropertyViewModel(string cellName, Guid cameraUid)
		{
			Cameras = new ObservableCollection<Camera>(FiresecManager.SystemConfiguration.Cameras);
			CellName = cellName;
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
