using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;

namespace VideoModule.ViewModels
{
	public class RviSettingsViewModel : SaveCancelDialogViewModel
	{
		public RviSettings RviSettings { get; private set; }

		public RviSettingsViewModel(RviSettings rviSettings)
		{
			Title = "Настройки";
			RviSettings = rviSettings;
			Ip = rviSettings.Ip;
			Port = rviSettings.Port;
			Login = rviSettings.Login;
			Password = rviSettings.Password;
			VideoWidth = rviSettings.VideoWidth.ToString();
			VideoHeight = rviSettings.VideoHeight.ToString();
			VideoMarginLeft = rviSettings.VideoMarginLeft.ToString();
			VideoMarginTop = rviSettings.VideoMarginTop.ToString();
		}

		string _ip;
		public string Ip
		{
			get { return _ip; }
			set
			{
				_ip = value;
				OnPropertyChanged(() => Ip);
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
		string _videoWidth;
		public string VideoWidth
		{
			get { return _videoWidth; }
			set
			{
				_videoWidth = value;
				OnPropertyChanged(() => VideoWidth);
			}
		}
		string _videoHeight;
		public string VideoHeight
		{
			get { return _videoHeight; }
			set
			{
				_videoHeight = value;
				OnPropertyChanged(() => VideoHeight);
			}
		}
		string _videoMarginLeft;
		public string VideoMarginLeft
		{
			get { return _videoMarginLeft; }
			set
			{
				_videoMarginLeft = value;
				OnPropertyChanged(() => VideoMarginLeft);
			}
		}
		string _videoMarginTop;
		public string VideoMarginTop
		{
			get { return _videoMarginTop; }
			set
			{
				_videoMarginTop = value;
				OnPropertyChanged(() => VideoMarginTop);
			}
		}
		protected override bool Save()
		{
			RviSettings.Ip = Ip;
			RviSettings.Port = Port;
			RviSettings.Login = Login;
			RviSettings.Password = Password;
			RviSettings.VideoWidth = int.Parse(VideoWidth);
			RviSettings.VideoHeight = int.Parse(VideoHeight);
			RviSettings.VideoMarginLeft = int.Parse(VideoMarginLeft);
			RviSettings.VideoMarginTop = int.Parse(VideoMarginTop);
			return base.Save();
		}
	}
}