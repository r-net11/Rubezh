using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public SKDReaderDUPropertyViewModel DUControl { get; private set; }
		public SKDReaderDUPropertyViewModel DUConversation { get; private set; }

		public ReaderDetailsViewModel(SKDDevice device)
		{
			Device = device;
			Title = "Свойства считывателя " + Device.Name;

			SelectCameraCommand = new RelayCommand(OnSelectCamera);
			RemoveCameraCommand = new RelayCommand(OnRemoveCamera, CanRemove);

			DUControl = new SKDReaderDUPropertyViewModel(Device.SKDReaderProperty.DUControl);
			DUConversation = new SKDReaderDUPropertyViewModel(Device.SKDReaderProperty.DUConversation);
			VerificationTime = Device.SKDReaderProperty.VerificationTime;

			Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == Device.CameraUID);
		}

		int _verificationTime;
		public int VerificationTime
		{
			get { return _verificationTime; }
			set
			{
				_verificationTime = value;
				OnPropertyChanged("VerificationTime");
			}
		}

		Camera _camera;
		public Camera Camera
		{
			get { return _camera; }
			set
			{
				_camera = value;
				OnPropertyChanged("Camera");
			}
		}

		public RelayCommand SelectCameraCommand { get; private set; }
		void OnSelectCamera()
		{
			var cameraSelectationViewModel = new CameraSelectationViewModel(Camera);
			if (DialogService.ShowModalWindow(cameraSelectationViewModel))
			{
				Camera = cameraSelectationViewModel.SelectedCamera;
			}
		}

		public RelayCommand RemoveCameraCommand { get; private set; }
		void OnRemoveCamera()
		{
			Camera = null;
		}
		bool CanRemove()
		{
			return Camera != null;
		}

		protected override bool Save()
		{
			DUControl.Save();
			DUConversation.Save();
			Device.SKDReaderProperty.VerificationTime = VerificationTime;
			Device.CameraUID = Camera == null ? Guid.Empty : Camera.UID;
			return true;
		}
	}

	public class SKDReaderDUPropertyViewModel : BaseViewModel
	{
		public SKDReaderDUProperty SKDReaderDUProperty { get; private set; }

		public SKDReaderDUPropertyViewModel(SKDReaderDUProperty skdReaderDUProperty)
		{
			SKDReaderDUProperty = skdReaderDUProperty;

			IsDU = SKDReaderDUProperty.IsDU;
			IsEmployeeDU = SKDReaderDUProperty.IsEmployeeDU;
			IsEmployeeDUTime = SKDReaderDUProperty.IsEmployeeDUTime;
			IsEmployeeDUZone = SKDReaderDUProperty.IsEmployeeDUZone;
			IsGuestDU = SKDReaderDUProperty.IsGuestDU;
			IsGuestDUTime = SKDReaderDUProperty.IsGuestDUTime;
			IsGuestDUZone = SKDReaderDUProperty.IsGuestDUZone;
		}

		bool _isDU;
		public bool IsDU
		{
			get { return _isDU; }
			set
			{
				_isDU = value;
				OnPropertyChanged("IsDU");
			}
		}

		bool _isEmployeeDU;
		public bool IsEmployeeDU
		{
			get { return _isEmployeeDU; }
			set
			{
				_isEmployeeDU = value;
				OnPropertyChanged("IsEmployeeDU");
			}
		}

		bool _isEmployeeDUTime;
		public bool IsEmployeeDUTime
		{
			get { return _isEmployeeDUTime; }
			set
			{
				_isEmployeeDUTime = value;
				OnPropertyChanged("IsEmployeeDUTime");
			}
		}

		bool _isEmployeeDUZone;
		public bool IsEmployeeDUZone
		{
			get { return _isEmployeeDUZone; }
			set
			{
				_isEmployeeDUZone = value;
				OnPropertyChanged("IsEmployeeDUZone");
			}
		}

		bool _isGuestDU;
		public bool IsGuestDU
		{
			get { return _isGuestDU; }
			set
			{
				_isGuestDU = value;
				OnPropertyChanged("IsGuestDU");
			}
		}

		bool _isGuestDUTime;
		public bool IsGuestDUTime
		{
			get { return _isGuestDUTime; }
			set
			{
				_isGuestDUTime = value;
				OnPropertyChanged("IsGuestDUTime");
			}
		}

		bool _isGuestDUZone;
		public bool IsGuestDUZone
		{
			get { return _isGuestDUZone; }
			set
			{
				_isGuestDUZone = value;
				OnPropertyChanged("IsGuestDUZone");
			}
		}

		public void Save()
		{
			SKDReaderDUProperty.IsDU = IsDU;
			SKDReaderDUProperty.IsEmployeeDU = IsEmployeeDU;
			SKDReaderDUProperty.IsEmployeeDUTime = IsEmployeeDUTime;
			SKDReaderDUProperty.IsEmployeeDUZone = IsEmployeeDUZone;
			SKDReaderDUProperty.IsGuestDU = IsGuestDU;
			SKDReaderDUProperty.IsGuestDUTime = IsGuestDUTime;
			SKDReaderDUProperty.IsGuestDUZone = IsGuestDUZone;
		}
	}
}