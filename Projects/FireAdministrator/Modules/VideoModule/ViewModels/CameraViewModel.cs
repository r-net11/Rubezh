using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Video;
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
		private MjpegCamera MjpegCamera { get; set; }
		public Camera Camera { get; set; }
		public string Error { get; private set; }
		public bool HasError { get; private set; }
		public CameraFramesWatcher CameraFramesWatcher { get; private set; }

		private bool _isNowPlaying;
		public bool IsNowPlaying
		{
			get { return _isNowPlaying; }
			private set
			{
				_isNowPlaying = value;
				OnPropertyChanged("IsNowPlaying");
			}
		}

		IEnumerable<Channel> _channels;
		public IEnumerable<Channel> Channels
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

		public CameraViewModel(CamerasViewModel camerasViewModel, Camera camera)
		{
			_camerasViewModel = camerasViewModel;
			Camera = camera;
			MjpegCamera = new MjpegCamera(camera);
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);
		}

		void GetError(string error)
		{
			Error = error;
			ImageSource = new BitmapImage();
		}

		void BmpToImageSource(Bitmap bmp)
		{
			using (var memory = new MemoryStream())
			{
				bmp.Save(memory, ImageFormat.Jpeg);
				memory.Position = 0;
				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
				{
					ImageSource = bitmapImage;
				}));
			}
		}

		public string PresentationZones
		{
			get
			{
				var zones = new List<XZone>();
				foreach (var zoneUID in Camera.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = XManager.GetCommaSeparatedZones(zones);
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

		Thread VideoThread { get; set; }

		#region MJPEG
		//public void StartVideo()
		//{
		//    //return; //TODO: TEST (CameraVideo isn't working now)
		//    MjpegCamera.FrameReady += BmpToImageSource;
		//    MjpegCamera.ErrorHandler += GetError;
		//    VideoThread = new Thread(MjpegCamera.StartVideo);
		//    VideoThread.Start();
		//    IsNowPlaying = true;
		//}
		//public void StopVideo()
		//{
		//    //TODO: TEST (CameraVideo isn't working now)
		//    //{
		//    //	IsNowPlaying = false;
		//    //	return;
		//    //}
		//    MjpegCamera.FrameReady -= BmpToImageSource;
		//    MjpegCamera.ErrorHandler -= GetError;
		//    MjpegCamera.StopVideo();
		//    ImageSource = new BitmapImage();
		//    IsNowPlaying = false;
		//}
		#endregion
		public void StartVideo()
		{
			try
			{
				CamerasView.Current.PlayerWrap.InitializeCamera(Camera);
				IsNowPlaying = true;
			}
			catch { }
		}

		public void StopVideo()
		{
			try
			{
				CamerasView.Current.PlayerWrap.InitializeCamera(new Camera());
				IsNowPlaying = false;
			}
			catch { }
		}

		private ImageSource _imageSource;
		public ImageSource ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged("ImageSource");
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
	}
}