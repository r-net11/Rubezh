using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public List<Guid> Zones { get; set; }
		CameraViewModel CameraViewModel { get; set; }
		public CameraViewModel OriginalCameraViewModel { get; private set; }

		public CameraDetailsViewModel(CameraViewModel cameraViewModel)
		{
			OriginalCameraViewModel = cameraViewModel;
			ShowZonesCommand = new RelayCommand(OnShowZones);
			ConnectCommand = new RelayCommand(OnConnect, () => !CanShow());
			ShowCommand = new RelayCommand(OnShow, CanShow);
			StateClasses = new List<XStateClass>();
			StateClasses.Add(XStateClass.Fire1);
			StateClasses.Add(XStateClass.Fire2);
			StateClasses.Add(XStateClass.Attention);
			StateClasses.Add(XStateClass.Ignore);

			if (cameraViewModel != null)
			{
				Channels = cameraViewModel.Channels;
				Title = "Редактировать камеру";
				CameraViewModel = CameraViewModel.CopyCameraViewModel(cameraViewModel);
			}
			else
			{
				Title = "Создать камеру";
				var camera = new Camera
				{
					Name = "Новая камера",
					Address = "172.16.7.88",
					Port = 37777,
					Login = "admin",
					Password = "admin"
				};
				CameraViewModel = new CameraViewModel(camera);
			}
			CopyProperties(CameraViewModel.Camera);
		}

		void CopyProperties(Camera camera)
		{
			Name = camera.Name;
			Address = camera.Address;
			Port = camera.Port;
			Login = camera.Login;
			Password = camera.Password;
			Channels = CameraViewModel.Channels;
			if (Channels != null)
				SelectedChannel = Channels[camera.ChannelNumber];
			Left = camera.Left;
			Top = camera.Top;
			Width = camera.Width;
			Height = camera.Height;
			IgnoreMoveResize = camera.IgnoreMoveResize;
			SelectedStateClass = camera.StateClass;
			if (camera.ZoneUIDs == null)
				camera.ZoneUIDs = new List<Guid>();
			Zones = camera.ZoneUIDs.ToList();
		}

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
				if(_selectedChannel != null)
					CameraViewModel.SelectedChannel = _selectedChannel;
				OnPropertyChanged(() => SelectedChannel);
			}
		}

		string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged("Login");
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged("Password");
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged("Address");
			}
		}

		int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged("Port");
			}
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged("Left");
			}
		}

		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged("Top");
			}
		}

		int _width;
		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged("Width");
			}
		}

		int _height;
		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged("Height");
			}
		}

		bool _ignoreMoveResize;
		public bool IgnoreMoveResize
		{
			get { return _ignoreMoveResize; }
			set
			{
				_ignoreMoveResize = value;
				OnPropertyChanged("IgnoreMoveResize");
			}
		}

		public string PresentationZones
		{
			get
			{
				var zones = Zones.Select(zoneUID => XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID)).Where(zone => zone != null).ToList();
				var presentationZones = XManager.GetCommaSeparatedObjects(new List<INamedBase>(zones));
				return presentationZones;
			}
		}

		public List<XStateClass> StateClasses { get; private set; }

		XStateClass _selectedStateClass;
		public XStateClass SelectedStateClass
		{
			get { return _selectedStateClass; }
			set
			{
				_selectedStateClass = value;
				OnPropertyChanged("SelectedStateClass");
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Zones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Zones = zonesSelectationViewModel.Zones;
				OnPropertyChanged("PresentationZones");
			}
		}

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			Copy(CameraViewModel);
			new Thread(delegate()
			{
				try
				{
					CameraViewModel.Connect();
					Channels = CameraViewModel.Channels;
					SelectedChannel = Channels.FirstOrDefault();
				}
				catch { }
			}).Start();
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			CameraViewModel.StartVideo();
		}

		bool CanShow()
		{
			if (IsChanged)
				return IsConnected = false;
			return IsConnected = CameraViewModel.IsConnected;
		}

		private bool _isConnected;
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				OnPropertyChanged(()=>IsConnected);
			}
		}

		bool IsChanged
		{
			get { return ((Address != CameraViewModel.Camera.Address) || (Port != CameraViewModel.Camera.Port)); }
		}

		protected override bool Save()
		{
			Copy(OriginalCameraViewModel);
			return base.Save();
		}

		void Copy(CameraViewModel cameraViewModel)
		{
			cameraViewModel.Camera.Name = Name;
			cameraViewModel.Camera.Address = Address;
			cameraViewModel.Camera.Port = Port;
			cameraViewModel.Camera.Login = Login;
			cameraViewModel.Camera.Password = Password;
			cameraViewModel.Channels = CameraViewModel.Channels;
			cameraViewModel.IsConnected = CameraViewModel.IsConnected;
			cameraViewModel.SelectedChannel = CameraViewModel.SelectedChannel;
			if (SelectedChannel != null)
				cameraViewModel.Camera.ChannelNumber = SelectedChannel.ChannelNumber;
			if (IsChanged)
			{
				cameraViewModel.Channels = null;
				cameraViewModel.SelectedChannel = null;
				cameraViewModel.IsConnected = false;
				cameraViewModel.Camera.ChannelNumber = 0;
			}
			cameraViewModel.Camera.Left = Left;
			cameraViewModel.Camera.Top = Top;
			cameraViewModel.Camera.Width = Width;
			cameraViewModel.Camera.Height = Height;
			cameraViewModel.Camera.StateClass = SelectedStateClass;
			cameraViewModel.Camera.ZoneUIDs = Zones.ToList();
			cameraViewModel.Camera.IgnoreMoveResize = IgnoreMoveResize;
		}
	}
}