using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Common;
using DeviceControls;
using Entities.DeviceOriented;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrustructure.Plans.Painters;
using Infrastructure.Common.Windows;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : TreeNodeViewModel<CameraViewModel>
	{
		public Camera Camera { get; set; }

		public CameraViewModel(Camera camera)
		{
			VisualCameraViewModels = new List<CameraViewModel>();
			Camera = camera;
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
		}

		public List<CameraViewModel> VisualCameraViewModels;

		private void PropertyChangedEvent(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			var device = sender as Device;
			var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.Ip == Camera.Ip);
			if ((device == null) || (camera == null))
			{
				throw new Exception("Неожидаемый null в CameraViewModel.PropertyChangedEvent");
			}
			var cameraViewModel = CamerasViewModel.Current.Cameras.FirstOrDefault(x => x.Camera.Ip == Camera.Ip);
			if (cameraViewModel != null)
				cameraViewModel.Update();
			if ((device.Status == DeviceStatuses.Disconnected) || (device.Status == DeviceStatuses.NotAvailable))
				StopAll();
			if (device.Status == DeviceStatuses.Connected)
				StartAll();
		}

		public string PresentationName
		{
			get
			{
				return Camera.Name + " " + Camera.Ip;
			}
		}

		public string PresentationAddress
		{
			get
			{
				return Camera.Ip;
			}
		}

		public string PresentationZones
		{
			get
			{
				var zones =
					Camera.ZoneUIDs.Select(zoneUID => GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID))
						.Where(zone => zone != null)
						.ToList();
				var presentationZones = GKManager.GetCommaSeparatedObjects(new List<ModelBase>(zones));
				return presentationZones;
			}
		}

		public string PresentationState
		{
			get
			{
				if ((Camera.StateClass == XStateClass.Fire1) || (Camera.StateClass == XStateClass.Fire2) ||
					(Camera.StateClass == XStateClass.Attention) || (Camera.StateClass == XStateClass.Ignore))
					return Camera.StateClass.ToDescription();
				return null;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationZones);
			OnPropertyChanged(() => PresentationAddress);
			OnPropertyChanged(() => PresentationState);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}

		public bool IsOnPlan
		{
			get { return Camera.PlanElementUIDs.Count > 0; }
		}

		public VisualizationState VisualizationState
		{
			get
			{
				return IsOnPlan
					? (Camera.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single)
					: VisualizationState.NotPresent;
			}
		}

		public void StartAll()
		{
			foreach (var visualCameraViewModel in VisualCameraViewModels)
			{
				visualCameraViewModel.Start(false);
			}
		}

		public void StopAll()
		{
			foreach (var visualCameraViewModel in VisualCameraViewModels)
			{
				visualCameraViewModel.Stop(false);
			}
		}

		public void Start(bool addToRootCamera = true)
		{
			//_cellPlayerWrap.Start(Camera, Camera.ChannelNumber);
			if ((addToRootCamera)&&(RootCamera != null))
				RootCamera.VisualCameraViewModels.Add(this);
		}

		public void Stop(bool addToRootCamera = true)
		{
			//_cellPlayerWrap.Stop();
			if ((addToRootCamera) && (RootCamera != null))
				RootCamera.VisualCameraViewModels.Remove(this);
		}

		CameraViewModel RootCamera
		{
			get
			{
				return CamerasViewModel.Current.Cameras.FirstOrDefault(x => x.Camera.Ip == Camera.Ip);
			}
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }

		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			var camera = Camera;
			dataObject.SetData("DESIGNER_ITEM", camera);
		}
		public Converter<IDataObject, UIElement> CreateDragVisual { get; private set; }

		private UIElement OnCreateDragVisual(IDataObject dataObject)
		{
			var brush = PictureCacheSource.CameraPicture.GetDefaultBrush();
			return new System.Windows.Shapes.Rectangle
			{
				Fill = brush,
				Height = PainterCache.DefaultPointSize,
				Width = PainterCache.DefaultPointSize,
			};
		}

		private void CellPlayerWrapOnDropHandler(Camera camera)
		{
			Camera = camera;
			try
			{
				ApplicationService.BeginInvoke(() =>
				{
					Stop();
					Start();
				});
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "LayoutMultiCameraView.OnShowProperties");
			}
		}

		private bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
		}
	}
}