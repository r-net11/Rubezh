using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		private string _connectedAddress;
		private int _connectedport;
		private string _connectedlogin;
		private string _connectedpassword;

		public List<XStateClass> StateClasses { get; private set; }
		public Camera Camera { get; set; }
		public CellPlayerWrap CellPlayerWrap { get; private set; }

		public CameraViewModel(Camera camera, CellPlayerWrap cellPlayerWrap)
		{
			Camera = camera;
			StateClasses = new List<XStateClass>();
			StateClasses.Add(XStateClass.Fire1);
			StateClasses.Add(XStateClass.Fire2);
			StateClasses.Add(XStateClass.Attention);
			StateClasses.Add(XStateClass.Ignore);
			CellPlayerWrap = cellPlayerWrap;
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

		public void ConnectAndStart()
		{
			try
			{
				IsConnecting = true;
				IsFailConnected = false;
				Channels = new ObservableCollection<Channel>(CellPlayerWrap.Connect(Camera.Address, Camera.Port));
				Dispatcher.BeginInvoke(
					DispatcherPriority.Input, new ThreadStart(
						() =>
						{
							SelectedChannel = Channels[Camera.ChannelNumber];
							StartVideo();
						}));
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
				if ((SelectedChannel == null)&&(Channels!=null))
					SelectedChannel = Channels.FirstOrDefault();
				CellPlayerWrap.Start(SelectedChannel.ChannelNumber);
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
				CellPlayerWrap.Stop();
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
					if ((SelectedChannel == null) && (Channels != null))
						SelectedChannel = Channels.FirstOrDefault();
					return true;
				}
				return false;
			}
		}

		public string Name
		{
			get { return Camera.Name; }
		}

		public string PresentationName
		{
			get { return Camera.Address + " (" + (Camera.ChannelNumber + 1) + " канал)"; }
		}

		public string Address
		{
			get { return Camera.Address; }
		}

		public int Port
		{
			get { return Camera.Port; }
		}

		public string Login
		{
			get { return Camera.Login; }
		}

		public string Password
		{
			get { return Camera.Password; }
		}

		public int Left
		{
			get { return Camera.Left; }
		}

		public int Top
		{
			get { return Camera.Top; }
		}

		public int Width
		{
			get { return Camera.Width; }
		}

		public int Height
		{
			get { return Camera.Height; }
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
		}

		public XStateClass SelectedStateClass
		{
			get { return Camera.StateClass; }
		}

		public List<Guid> ZoneUIDs
		{
			get { return Camera.ZoneUIDs; }
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
	}
}