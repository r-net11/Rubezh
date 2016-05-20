using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using WPFMediaKit.DirectShow.Interop;

namespace SKDModule.ViewModels
{
	class WebCameraDetailsViewModel : DialogViewModel
	{
		public WebCameraDetailsViewModel()
		{
			Title = "Web-камера";
			OkCommand = new RelayCommand(OnOk);
			CancelCommand = new RelayCommand(OnCancel);
		}

		public byte[] Data { get; set; }

		public RelayCommand OkCommand { get; set; }
		void OnOk()
		{
			Close(true);
		}

		public RelayCommand CancelCommand { get; set; }
		void OnCancel()
		{
			Close(false);
		}

		public DsDevice VideoDevicesList { get { return WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices.FirstOrDefault<DsDevice>(); } }
	}
}