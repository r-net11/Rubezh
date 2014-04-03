using System.Threading;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public CameraDetailsViewModel(Camera camera)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			ConnectCommand = new RelayCommand(OnConnect);
			ShowCommand = new RelayCommand(OnShow);

			if (camera == null)
			{
				Title = "Создать камеру";
				camera = new Camera();
			}
			else
			{
				Title = "Редактировать камеру";
			}

			CopyProperties(camera);
		}

		void CopyProperties(Camera camera)
		{
			Name = camera.Name;
			Address = camera.Address;
			Login = camera.Login;
			Name = camera.Name;
			Name = camera.Name;
			Name = camera.Name;
			Name = camera.Name;
			Name = camera.Name;
			Name = camera.Name;
		}

		private CameraViewModel _cameraViewModel;
		public CameraViewModel CameraViewModel
		{
			get { return _cameraViewModel; }
			set
			{
				_cameraViewModel = value;
				OnPropertyChanged(() => CameraViewModel);
			}
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

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(CameraViewModel.ZoneUIDs);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				CameraViewModel.ZoneUIDs = zonesSelectationViewModel.Zones;
				OnPropertyChanged("CameraViewModel");
				OnPropertyChanged("CameraViewModel.PresentationZones");
			}
		}

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			new Thread(delegate()
			{
				try
				{
					CameraViewModel.Connect();
				}
				catch { }
			}).Start();
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			CameraViewModel.StartVideo();
		}

		protected override bool Save()
		{
			OriginalCameraViewModel = CameraViewModel;
			return base.Save();
		}
	}
}