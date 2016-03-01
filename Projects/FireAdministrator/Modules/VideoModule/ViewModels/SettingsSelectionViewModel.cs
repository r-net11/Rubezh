using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class SettingsSelectionViewModel : SaveCancelDialogViewModel
	{
		private VideoIntegrationProvider _oldVideoIntegrationProvider;

		public RviSettings RviSettings { get; private set; }

		public SettingsSelectionViewModel(RviSettings rviSettings)
		{
			Title = "Настройки";

			_oldVideoIntegrationProvider = rviSettings.VideoIntegrationProvider;
			RviSettings = new RviSettings();
			VideoIntegrationProvider = rviSettings.VideoIntegrationProvider;
			Ip = rviSettings.Ip;
			Port = rviSettings.Port;
			Login = rviSettings.Login;
			Password = rviSettings.Password;

			VideoIntegrationProviders = new ObservableCollection<VideoIntegrationProvider>
			{
				VideoIntegrationProvider.RviOperator,
				VideoIntegrationProvider.RviIntegrator
			};
		}

		public ObservableCollection<VideoIntegrationProvider> VideoIntegrationProviders { get; private set; }

		private VideoIntegrationProvider _videoIntegrationProvider;
		public VideoIntegrationProvider VideoIntegrationProvider
		{
			get { return _videoIntegrationProvider; }
			set
			{
				if (_videoIntegrationProvider == value)
					return;
				_videoIntegrationProvider = value;
				OnPropertyChanged(() => VideoIntegrationProvider);
			}
		}

		private string _ip;
		public string Ip
		{
			get { return _ip; }
			set
			{
				_ip = value;
				OnPropertyChanged(() => Ip);
			}
		}

		private int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		private string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		protected override bool Save()
		{
			RviSettings.VideoIntegrationProvider = VideoIntegrationProvider;
			RviSettings.Ip = Ip;
			RviSettings.Port = Port;
			RviSettings.Login = Login;
			RviSettings.Password = Password;

			if (VideoIntegrationProvider != _oldVideoIntegrationProvider)
				return MessageBoxService.ShowConfirmation("Изменение типа сервера влечет удаление ранее добавленных камер. Сохранить настройки и удалить камеры?");
			
			return base.Save();
		}
	}
}
