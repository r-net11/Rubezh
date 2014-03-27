using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Entities.DeviceOriented;
using Entities.DeviceOriented.Dahua;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using VideoModule.Views;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		public List<Guid> Zones { get; set; }

		public CameraDetailsViewModel(CameraViewModel cameraViewModel = null)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			TestCommand = new RelayCommand(OnTest);
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
				Camera = cameraViewModel.Camera;
			}
			else
			{
				Title = "Создать камеру";
				Camera = new Camera
				{
					Name = "Новая камера",
					Address = "172.16.7.88",
					Port = 37777,
					Login = "admin",
					Password = "admin"
				};
			}

			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Camera.Name;
			Address = Camera.Address;
			Port = Camera.Port;
			Login = Camera.Login;
			Password = Camera.Password;
			if (Channels != null)
				SelectedChannel = Channels[Camera.ChannelNumber];
			Left = Camera.Left;
			Top = Camera.Top;
			Width = Camera.Width;
			Height = Camera.Height;
			IgnoreMoveResize = Camera.IgnoreMoveResize;
			SelectedStateClass = Camera.StateClass;
			if (Camera.ZoneUIDs == null)
				Camera.ZoneUIDs = new List<Guid>();
			Zones = Camera.ZoneUIDs.ToList();
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
				var zones = new List<XZone>();
				foreach (var zoneUID in Zones)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = XManager.GetCommaSeparatedZones(zones);
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
			new Thread(delegate()
			{
				var deviceSI = new DeviceSearchInfo(Address, Port);
				try
				{
					var device = SystemPerimeter.Instance.AddDevice(deviceSI);
					Channels = new ObservableCollection<Channel>(device.Channels);
					SelectedChannel = Channels.FirstOrDefault();
					Save();
				}
				catch { }
			}).Start();
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			var title = Camera.Address + " (" + SelectedChannel.Name +")";
			var previewViewModel = new PreviewViewModel(title);
			DialogService.ShowWindow(previewViewModel);
			Save();
			var cameraViewModel = new CameraViewModel(Camera);
			cameraViewModel.StartVideo(PreviewView.Current.PlayerWrap);
		}

		bool CanShow()
		{
			return ((Address == Camera.Address) && (Port == Camera.Port) && (SelectedChannel != null));
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			var camera = new Camera
			{
				Address = Address,
				Port = Port,
				Login = Login,
				Password = Password,
				Left = Left,
				Top = Top,
				Width = Width,
				Height = Height,
				IgnoreMoveResize = IgnoreMoveResize
			};
			VideoService.ShowModal(camera);
			Left = camera.Left;
			Top = camera.Top;
			Width = camera.Width;
			Height = camera.Height;
		}

		protected override bool Save()
		{
			Camera.Name = Name;
			Camera.Address = Address;
			Camera.Port = Port;
			Camera.Login = Login;
			Camera.Password = Password;
			if (SelectedChannel != null)
				Camera.ChannelNumber = SelectedChannel.ChannelNumber;
			Camera.Left = Left;
			Camera.Top = Top;
			Camera.Width = Width;
			Camera.Height = Height;
			Camera.StateClass = SelectedStateClass;
			Camera.ZoneUIDs = Zones.ToList();
			Camera.IgnoreMoveResize = IgnoreMoveResize;
			return base.Save();
		}

		protected override bool CanSave()
		{
			return ((Camera.Name != Name) || (Camera.Address != Address) || (Camera.Port != Port) ||
			        (Camera.Login != Login) || (Camera.Password != Password) ||
			        (Camera.ChannelNumber != SelectedChannel.ChannelNumber) ||
			        (Camera.Left != Left) || (Camera.Top != Top) || (Camera.Width != Width) || (Camera.Height != Height) ||
			        (Camera.StateClass != SelectedStateClass) || (!Camera.ZoneUIDs.SequenceEqual(Zones.ToList())) ||
			        (Camera.IgnoreMoveResize != IgnoreMoveResize));
		}
	}
}