using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using WPFMediaKit.DirectShow.Interop;
using System.Windows.Media.Imaging;
using System.IO;


namespace SKDModule.ViewModels
{
	class WebCameraDetailsViewModel : DialogViewModel
	{
		public WebCameraDetailsViewModel()
		{
			OkCommand = new RelayCommand(OnOk);
			CancelCommand = new RelayCommand(OnCancel);
			YesCommand = new RelayCommand(OnYes);
			NoCommand = new RelayCommand(OnNo);
			TakeSnapshotCommand = new RelayCommand(OnTakeSnapshot);
		}

		private void OnOk()
		{
			throw new NotImplementedException();
		}

		private void OnCancel()
		{
			throw new NotImplementedException();
		}

		private void OnYes()
		{
			throw new NotImplementedException();
		}

		private void OnNo()
		{
			throw new NotImplementedException();
		}

		public RelayCommand OkCommand { get; set; }

		public RelayCommand CancelCommand { get; set; }

		public RelayCommand YesCommand { get; set; }

		public RelayCommand NoCommand { get; set; }

		public RelayCommand TakeSnapshotCommand { get; set; }

		public DsDevice VideoDevicesList { get { return WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices.FirstOrDefault<DsDevice>(); } }
		void OnTakeSnapshot()
		{

		}
		public static bool HasCamera { 
			get 
			{
				return WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices.Length > 0; 
			} 
		}
	}
}
