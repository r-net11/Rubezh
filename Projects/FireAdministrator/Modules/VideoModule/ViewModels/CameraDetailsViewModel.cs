using System.Threading;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public CameraViewModel OriginalCameraViewModel { get; private set; }

		public CameraDetailsViewModel(CameraViewModel cameraViewModel)
		{
			OriginalCameraViewModel = cameraViewModel;

			ShowZonesCommand = new RelayCommand(OnShowZones);
			ConnectCommand = new RelayCommand(OnConnect);
			ShowCommand = new RelayCommand(OnShow);

			if (cameraViewModel != null)
			{
				Title = "Редактировать камеру";
				CameraViewModel = new CameraViewModel(new Camera());
				CameraViewModel.CopyCameraViewModel(cameraViewModel);
			}
			else
			{
				Title = "Создать камеру";
				CameraViewModel = CameraViewModel.CreateDefaultCamera();
			}
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