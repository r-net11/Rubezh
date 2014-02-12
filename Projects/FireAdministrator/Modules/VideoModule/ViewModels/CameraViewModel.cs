using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Video;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		private MjpegCamera MjpegCamera { get; set; }
		public Camera Camera { get; set; }
		public List<StringBuilder> ErrorLog { get; private set; }
		public bool HasError { get; private set; }
		public CameraFramesWatcher CameraFramesWatcher { get; private set; }

		public bool IsNowPlaying { get; private set; }
		public CameraViewModel(Camera camera)
		{
			Camera = camera;
			ErrorLog = new List<StringBuilder>();
			MjpegCamera = new MjpegCamera(camera);
		}
		void GetError(string error)
		{
			ErrorLog.Add(new StringBuilder(error));
			ImageSource = new BitmapImage();
		}

		void BmpToImageSource(Bitmap bmp)
		{
			using (var memory = new MemoryStream())
			{
				bmp.Save(memory, ImageFormat.Jpeg);
				memory.Position = 0;
				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
				{
					ImageSource = bitmapImage;
				}));
			}
		}

		public string PresentationZones
		{
			get
			{
				var zones = new List<XZone>();
				foreach (var zoneUID in Camera.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = XManager.GetCommaSeparatedZones(zones);
				return presentationZones;
			}
		}

		public void Update()
		{
			OnPropertyChanged("Camera");
			OnPropertyChanged("PresentationZones");
		}

		Thread VideoThread { get; set; }
		public void StartVideo()
		{
			//return; //TODO: TEST (Camera isn't working now)
			MjpegCamera.FrameReady += BmpToImageSource;
			MjpegCamera.ErrorHandler += GetError;
			VideoThread = new Thread(MjpegCamera.StartVideo);
			VideoThread.Start();
			IsNowPlaying = true;
			OnPropertyChanged("IsNowPlaying");
		}
		public void StopVideo()
		{
			//TODO: TEST (Camera isn't working now)
			//{
			//    IsNowPlaying = false;
			//    return;
			//}
			MjpegCamera.FrameReady -= BmpToImageSource;
			MjpegCamera.ErrorHandler -= GetError;
			MjpegCamera.StopVideo();
			ImageSource = new BitmapImage();
			IsNowPlaying = false;
			OnPropertyChanged("IsNowPlaying");
		}

		private ImageSource _imageSource;
		public ImageSource ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged("ImageSource");
			}
		}
	}
}