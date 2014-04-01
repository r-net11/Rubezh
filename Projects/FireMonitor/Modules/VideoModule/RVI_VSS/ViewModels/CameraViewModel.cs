using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace VideoModule.RVI_VSS.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		private string _connectedAddress;
		private int _connectedport;
		private string _connectedlogin;
		private string _connectedpassword;

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

		public string PresentationName
		{
			get { return Camera.Address + " (" + (Camera.ChannelNumber + 1) + " канал)"; }
		}

		public string Address
		{
			get { return Camera.Address; }
			set
			{
				Camera.Address = value;
				OnPropertyChanged(() => Address);
			}
		}

		public int Port
		{
			get { return Camera.Port; }
			set
			{
				Camera.Port = value;
				OnPropertyChanged(() => Port);
			}
		}

		public string Login
		{
			get { return Camera.Login; }
			set
			{
				Camera.Login = value;
				OnPropertyChanged(() => Login);
			}
		}

		public string Password
		{
			get { return Camera.Password; }
			set
			{
				Camera.Password = value;
				OnPropertyChanged(() => Password);
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
				var presentationZones = XManager.GetCommaSeparatedZones(zones);
				return presentationZones;
			}
		}
	}
}