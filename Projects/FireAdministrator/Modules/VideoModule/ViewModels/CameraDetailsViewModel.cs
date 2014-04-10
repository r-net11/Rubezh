using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using FiresecClient;
using System.Collections.Generic;
using XFiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common.Video.RVI_VSS;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		CellPlayerWrap _cellPlayerWrap;

		public CameraDetailsViewModel(Camera camera = null)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			RefreshCommand = new RelayCommand(OnRefresh);
			ShowCommand = new RelayCommand(OnShow);

			if (camera == null)
			{
				Title = "Создание новой камеры";
				Camera = new Camera();
			}
			else
			{
				Camera = camera;
				Title = "Свойства камеры: " + Camera.PresentationName;
			}
			_cellPlayerWrap = new CellPlayerWrap();

			Initialize();
			CopyProperties();
		}

		void Initialize()
		{
			StateClasses = new List<XStateClass>();
			StateClasses.Add(XStateClass.Fire1);
			StateClasses.Add(XStateClass.Fire2);
			StateClasses.Add(XStateClass.Attention);
			StateClasses.Add(XStateClass.Ignore);

			Channels = new ObservableCollection<ChannelViewModel>();
			for (int i = 0; i < 16; i++)
			{
				var channelViewModel = new ChannelViewModel(i);
				Channels.Add(channelViewModel);
			}
		}

		void CopyProperties()
		{
			Name = Camera.Name;
			Address = Camera.Address;
			Port = Camera.Port;
			Login = Camera.Login;
			Password = Camera.Password;
			Left = Camera.Left;
			Top = Camera.Top;
			Width = Camera.Width;
			Height = Camera.Height;
			IgnoreMoveResize = Camera.IgnoreMoveResize;
			SelectedStateClass = Camera.StateClass;
			SelectedChannel = Channels.FirstOrDefault(x => x.No == Camera.ChannelNumber);
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}

		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		int _width;
		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		int _height;
		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		bool _ignoreMoveResize;
		public bool IgnoreMoveResize
		{
			get { return _ignoreMoveResize; }
			set
			{
				_ignoreMoveResize = value;
				OnPropertyChanged(() => IgnoreMoveResize);
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
				OnPropertyChanged(() => SelectedStateClass);
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Camera.ZoneUIDs);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Camera.ZoneUIDs = zonesSelectationViewModel.Zones;
				OnPropertyChanged("PresentationZones");
			}
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

		#region Connection

		private ObservableCollection<ChannelViewModel> _channels;
		public ObservableCollection<ChannelViewModel> Channels
		{
			get { return _channels; }
			set
			{
				_channels = value;
				OnPropertyChanged(() => Channels);
			}
		}

		private ChannelViewModel _selectedChannel;
		public ChannelViewModel SelectedChannel
		{
			get { return _selectedChannel; }
			set
			{
				_selectedChannel = value;
				OnPropertyChanged(() => SelectedChannel);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			try
			{
				Channels = new ObservableCollection<ChannelViewModel>();
				var channels = _cellPlayerWrap.Connect(Address, Port, Login, Password);
				if (channels != null)
				{
					for (int i = 0; i < channels.Count; i++)
					{
						var channelViewModel = new ChannelViewModel(i, channels[i]);
						Channels.Add(channelViewModel);
					}
				}
				else
				{
					Channels = new ObservableCollection<ChannelViewModel>();
					for (int i = 0; i < 16; i++)
					{
						var channelViewModel = new ChannelViewModel(i);
						Channels.Add(channelViewModel);
					}
				}
				SelectedChannel = Channels.FirstOrDefault(x => x.No == Camera.ChannelNumber);
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			try
			{
				if (SelectedChannel == null)
					SelectedChannel = Channels.FirstOrDefault();
				if (SelectedChannel == null)
					return;
				var title = Address + " (" + SelectedChannel.Name + ")";
				var previewViewModel = new PreviewViewModel(title, _cellPlayerWrap);

				_cellPlayerWrap.Connect(Address, Port, Login, Password);
				_cellPlayerWrap.Start(SelectedChannel.No);
				DialogService.ShowModalWindow(previewViewModel);
				_cellPlayerWrap.Stop();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		#endregion

		protected override bool Save()
		{
			Camera.Name = Name;
			Camera.Address = Address;
			Camera.Port = Port;
			Camera.Login = Login;
			Camera.Password = Password;
			if (SelectedChannel != null)
				Camera.ChannelNumber = SelectedChannel.No;
			else
				Camera.ChannelNumber = 0;
			Camera.Left = Left;
			Camera.Top = Top;
			Camera.Width = Width;
			Camera.Height = Height;
			Camera.IgnoreMoveResize = IgnoreMoveResize;
			Camera.StateClass = SelectedStateClass;

			_cellPlayerWrap.Stop();

			return base.Save();
		}
	}
}