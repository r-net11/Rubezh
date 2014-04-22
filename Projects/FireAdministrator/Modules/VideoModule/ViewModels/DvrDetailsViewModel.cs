using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	class DvrDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }

		public DvrDetailsViewModel(Camera camera = null)
		{
			if (camera == null)
			{
				Title = "Создание нового видеорегистратора";
				Camera = new Camera();
				Camera.Name = "Видеорегистратор";
				Camera.Address = "172.16.5.201";
				ChannelsCount = 1;
				Camera.CameraType = XCameraType.Dvr;
				IsChannelsCountVisible = true;
			}
			else
			{
				Camera = camera;
				Title = "Свойства видеорегистратора: " + Camera.PresentationName;
				IsChannelsCountVisible = false;
			}
			CopyProperties();
		}

		private bool _isCamera;
		public bool IsCamera
		{
			get { return _isCamera; }
			set
			{
				_isCamera = value;
				IsChannelsCountVisible = !_isCamera;
				OnPropertyChanged(()=>IsCamera);
			}
		}

		public bool IsChannelsCountVisible { get; private set; }

		void CopyProperties()
		{
			Name = Camera.Name;
			Address = Camera.Address;
			Port = Camera.Port;
			Login = Camera.Login;
			Password = Camera.Password;
			IsCamera = Camera.CameraType == XCameraType.Camera;
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

		int _channelsCount;
		public int ChannelsCount
		{
			get { return _channelsCount; }
			set
			{
				_channelsCount = value;
				OnPropertyChanged(() => ChannelsCount);
			}
		}

		protected override bool Save()
		{
			Camera.Name = Name;
			Camera.Address = Address;
			Camera.Port = Port;
			Camera.Login = Login;
			Camera.Password = Password;
			Camera.CameraType = IsCamera ? XCameraType.Camera : XCameraType.Dvr;
			if(!IsCamera)
			for (int i = 0; i < ChannelsCount; i++)
			{
				Camera.Children.Add(new Camera{ChannelNumber = i, Parent = Camera});
			}
			return base.Save();
		}
	}
}
