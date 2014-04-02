using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
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
		private string _connectedAddress;
		private int _connectedport;
		private string _connectedlogin;
		private string _connectedpassword;
		private CamerasViewModel _camerasViewModel;
		private CellPlayerWrap _cellPlayerWrap;

		public List<XStateClass> StateClasses { get; private set; }
		public Camera Camera { get; set; }

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
			StateClasses = new List<XStateClass>();
			StateClasses.Add(XStateClass.Fire1);
			StateClasses.Add(XStateClass.Fire2);
			StateClasses.Add(XStateClass.Attention);
			StateClasses.Add(XStateClass.Ignore);
			_cellPlayerWrap = new CellPlayerWrap();
		}

		public CameraViewModel(CamerasViewModel camerasViewModel, Camera camera)
		{
			_camerasViewModel = camerasViewModel;
			_cellPlayerWrap = new CellPlayerWrap();
			Camera = camera;
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand,
				CanAllowMultipleVizualizationCommand);
		}

		public string Name
		{
			get { return Camera.Name; }
			set
			{
				Camera.Name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public string Address
		{
			get { return Camera.Address; }
			set
			{
				Camera.Address = value;
				OnPropertyChanged(() => Address);
				OnPropertyChanged(() => IsConnected);
			}
		}

		public int Port
		{
			get { return Camera.Port; }
			set
			{
				Camera.Port = value;
				OnPropertyChanged(() => Port);
				OnPropertyChanged(() => IsConnected);
			}
		}

		public string Login
		{
			get { return Camera.Login; }
			set
			{
				Camera.Login = value;
				OnPropertyChanged(() => Login);
				OnPropertyChanged(() => IsConnected);
			}
		}

		public string Password
		{
			get { return Camera.Password; }
			set
			{
				Camera.Password = value;
				OnPropertyChanged(() => Password);
				OnPropertyChanged(() => IsConnected);
			}
		}

		public int Left
		{
			get { return Camera.Left; }
			set
			{
				Camera.Left = value;
				OnPropertyChanged(() => Left);
			}
		}

		public int Top
		{
			get { return Camera.Top; }
			set
			{
				Camera.Top = value;
				OnPropertyChanged(() => Top);
			}
		}

		public int Width
		{
			get { return Camera.Width; }
			set
			{
				Camera.Width = value;
				OnPropertyChanged(() => Width);
			}
		}

		public int Height
		{
			get { return Camera.Height; }
			set
			{
				Camera.Height = value;
				OnPropertyChanged(() => Height);
			}
		}

		private ObservableCollection<Channel> _channels;

		public ObservableCollection<Channel> Channels
		{
			get { return _channels; }
			set
			{
				_channels = value;
				OnPropertyChanged(() => Channels);
			}
		}

		private Channel _selectedChannel;
		public Channel SelectedChannel
		{
			get { return _selectedChannel; }
			set
			{
				_selectedChannel = value;
				OnPropertyChanged(() => SelectedChannel);
			}
		}

		public bool IgnoreMoveResize
		{
			get { return Camera.IgnoreMoveResize; }
			set
			{
				Camera.IgnoreMoveResize = value;
				OnPropertyChanged(() => IgnoreMoveResize);
			}
		}

		public XStateClass SelectedStateClass
		{
			get { return Camera.StateClass; }
			set
			{
				Camera.StateClass = value;
				OnPropertyChanged(() => SelectedStateClass);
			}
		}

		public List<Guid> ZoneUIDs
		{
			get { return Camera.ZoneUIDs; }
			set { Camera.ZoneUIDs = value; }
		}

		public string PresentationZones
		{
			get
			{
				var zones =
					Camera.ZoneUIDs.Select(zoneUID => XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID))
						.Where(zone => zone != null)
						.ToList();
				var presentationZones = XManager.GetCommaSeparatedObjects(new List<INamedBase>(zones));
				return presentationZones;
			}
		}

		private bool _isConnecting;

		public bool IsConnecting
		{
			get { return _isConnecting; }
			set
			{
				_isConnecting = value;
				OnPropertyChanged(() => IsConnecting);
			}
		}

		private bool _isFailConnected;

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
				IsFailConnected = false;
				Channels = new ObservableCollection<Channel>(_cellPlayerWrap.Connect(Camera.Address, Camera.Port));
				Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => SelectedChannel = Channels[Camera.ChannelNumber]));
				SynchronizeProperties();
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
				if (SelectedChannel == null)
					SelectedChannel = Channels.FirstOrDefault();
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

		void SynchronizeProperties()
		{
			_connectedAddress = Address;
			_connectedport = Port;
			_connectedlogin = Login;
			_connectedpassword = Password;
			OnPropertyChanged(() => IsConnected);
		}

		public bool IsConnected
		{
			get
			{
				if (((_connectedAddress == Address) && (_connectedport == Port)) &&
				    ((_connectedlogin == Login) && (_connectedpassword == Password)))
				{
					if ((SelectedChannel == null)&&(Channels != null))
						SelectedChannel = Channels.FirstOrDefault();
					return true;
				}
				return false;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationZones);
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

		private void OnShowOnPlan()
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

		public void CopyCameraViewModel(CameraViewModel cameraViewModel)
		{
			Name = cameraViewModel.Camera.Name;
			Address = cameraViewModel.Camera.Address;
			Port = cameraViewModel.Camera.Port;
			Login = cameraViewModel.Camera.Login;
			Password = cameraViewModel.Camera.Password;
			Channels = cameraViewModel.Channels;
			if (cameraViewModel.IsConnected)
				SelectedChannel = cameraViewModel.SelectedChannel;
			else
			{
				SelectedChannel = null;
			}
			Left = cameraViewModel.Camera.Left;
			Top = cameraViewModel.Camera.Top;
			Width = cameraViewModel.Camera.Width;
			Height = cameraViewModel.Camera.Height;
			IgnoreMoveResize = cameraViewModel.IgnoreMoveResize;
			SelectedStateClass = cameraViewModel.Camera.StateClass;
			ZoneUIDs = cameraViewModel.Camera.ZoneUIDs;
			_cellPlayerWrap = cameraViewModel._cellPlayerWrap;
			_connectedAddress = cameraViewModel._connectedAddress;
			_connectedport = cameraViewModel._connectedport;
			_connectedlogin = cameraViewModel._connectedlogin;
			_connectedpassword = cameraViewModel._connectedpassword;
		}

		public static CameraViewModel CreateDefaultCamera()
		{
			var camera = new Camera
			{
				Name = "Новая камера",
				Address = "172.16.7.88",
				Port = 37777,
				Login = "admin",
				Password = "admin"
			};
			return new CameraViewModel(camera);
		}
	}
}