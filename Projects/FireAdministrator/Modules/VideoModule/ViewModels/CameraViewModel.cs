using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Video;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using VideoModule.Views;
using XFiresecAPI;
using System.Windows;
using DeviceControls;
using Infrustructure.Plans.Painters;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Events;
using System.Windows.Input;
using Infrastructure;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		private CamerasViewModel _camerasViewModel;
		private CellPlayerWrap _cellPlayerWrap;
		public Camera Camera { get; set; }
		public string Error { get; private set; }
		public bool HasError { get; private set; }
		public CameraFramesWatcher CameraFramesWatcher { get; private set; }

		ObservableCollection<Channel> _channels;
		public ObservableCollection<Channel> Channels
		{
			get { return _channels; }
			set
			{
				_channels = value;
				OnPropertyChanged(() => Channels);
			}
		}

		Channel _selectedChannel;
		public Channel SelectedChannel
		{
			get { return _selectedChannel; }
			set
			{
				_selectedChannel = value;
				OnPropertyChanged(() => SelectedChannel);
			}
		}

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
			_cellPlayerWrap = new CellPlayerWrap();
		}

		public CameraViewModel(CamerasViewModel camerasViewModel, Camera camera)
		{
			_camerasViewModel = camerasViewModel;
			_cellPlayerWrap = new CellPlayerWrap();
			Camera = camera;
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);
		}
		
		public string PresentationZones
		{
			get
			{
				var zones = Camera.ZoneUIDs.Select(zoneUID => XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID)).Where(zone => zone != null).ToList();
				var presentationZones = XManager.GetCommaSeparatedObjects(new List<INamedBase>(zones));
				return presentationZones;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationZones);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}

		bool _isConnecting;
		public bool IsConnecting
		{
			get { return _isConnecting; }
			set
			{
				_isConnecting = value;
				OnPropertyChanged(() => IsConnecting);
			}
		}

		bool _isConnected;
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				OnPropertyChanged(() => IsConnected);
			}
		}

		bool _isFailConnected;
		public bool IsFailConnected
		{
			get { return _isFailConnected; }
			set
			{
				_isFailConnected = value;
				OnPropertyChanged(() => IsFailConnected);
			}
		}
		
		public void Connect()
		{
			try
			{
				IsConnecting = true;
				IsConnected = false;
				IsFailConnected = false;
				Channels = new ObservableCollection<Channel>(_cellPlayerWrap.Connect(Camera.Address, Camera.Port));
				SelectedChannel = Channels[Camera.ChannelNumber];
				IsConnected = true;
			}
			catch
			{
				IsFailConnected = true;
			}
			finally
			{
				IsConnecting = false;
			}
		}
		
		public bool StartVideo()
		{
			try
			{
				var title = Camera.Address + " (" + SelectedChannel.Name + ")";
				var previewViewModel = new PreviewViewModel(title, _cellPlayerWrap);
				_cellPlayerWrap.Start(SelectedChannel.ChannelNumber);
				DialogService.ShowModalWindow(previewViewModel);
				_cellPlayerWrap.Stop();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool StopVideo()
		{
			try
			{
				_cellPlayerWrap.Stop();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool IsOnPlan
		{
			get { return Camera.PlanElementUIDs.Count > 0; }
		}

		public VisualizationState VisualizationState
		{
			get { return IsOnPlan ? (Camera.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent; }
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			_camerasViewModel.SelectedCamera = this;
			var plansElement = new ElementCamera
			{
				CameraUID = Camera.UID
			};
			dataObject.SetData("DESIGNER_ITEM", plansElement);
		}

		private bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
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

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Camera.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Camera.PlanElementUIDs);
		}

		public RelayCommand<bool> AllowMultipleVizualizationCommand { get; private set; }
		private void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Camera.AllowMultipleVizualization = isAllow;
			Update();
			CommandManager.InvalidateRequerySuggested();
			ServiceFactory.SaveService.CamerasChanged = true;
		}

		private bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Camera.AllowMultipleVizualization != isAllow;
		}
		
		public static CameraViewModel CopyCameraViewModel(CameraViewModel cameraViewModel)
		{
			var camera = new Camera();
			camera.Name = cameraViewModel.Camera.Name;
			camera.Address = cameraViewModel.Camera.Address;
			camera.Port = cameraViewModel.Camera.Port;
			camera.Login = cameraViewModel.Camera.Login;
			camera.Password = cameraViewModel.Camera.Password;
			camera.ChannelNumber = cameraViewModel.Camera.ChannelNumber;

			camera.Left = cameraViewModel.Camera.Left;
			camera.Top = cameraViewModel.Camera.Top;
			camera.Width = cameraViewModel.Camera.Width;
			camera.Height = cameraViewModel.Camera.Height;
			camera.IgnoreMoveResize = cameraViewModel.Camera.IgnoreMoveResize;
			camera.StateClass = cameraViewModel.Camera.StateClass;
			camera.ZoneUIDs = cameraViewModel.Camera.ZoneUIDs;

			var newCameraViewModel = new CameraViewModel(camera);
			if (cameraViewModel.Channels != null)
			{
				newCameraViewModel.Channels = new ObservableCollection<Channel>(cameraViewModel.Channels);
				newCameraViewModel.SelectedChannel = newCameraViewModel.Channels[newCameraViewModel.Camera.ChannelNumber];
			}
			newCameraViewModel.IsConnected = cameraViewModel.IsConnected;
			newCameraViewModel.IsConnecting = cameraViewModel.IsConnecting;
			newCameraViewModel.IsFailConnected = cameraViewModel.IsFailConnected;
			newCameraViewModel._cellPlayerWrap = cameraViewModel._cellPlayerWrap;
			return newCameraViewModel;
		}
	}
}