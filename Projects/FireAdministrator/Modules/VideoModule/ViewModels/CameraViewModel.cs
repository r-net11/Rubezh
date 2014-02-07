using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using mjpeg;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		private const int IMAGES_BUFFER_SIZE = 10;
		private int ImagesBufferIndex = 0;
		private MjpegCamera MjpegCamera { get; set; }
		public Camera Camera { get; set; }
		public List<StringBuilder> ErrorLog { get; private set; }
		public bool HasError { get; private set; }
		public List<CameraFrameWatcher> CameraFramesWatcher { get; private set; }

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
			ErrorLog = new List<StringBuilder>();
			MjpegCamera = new MjpegCamera(camera.Address, camera.Login, camera.Password);
			CameraFramesWatcher = new List<CameraFrameWatcher>(IMAGES_BUFFER_SIZE);
			InitializeCameraFramesWatcher();
		}
		public bool IsNowPlaying { get; private set; }
		void GetError(string error)
		{
			ErrorLog.Add(new StringBuilder(error));
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

		private void BmpToCameraFramesWatcher(Bitmap bmp)
		{
			var dateTime = DateTime.Now;
			if (dateTime - CameraFramesWatcher.Max().DateTime > TimeSpan.FromSeconds(1))
			{
				var cameraFrameWatcher = new CameraFrameWatcher(bmp, dateTime);
				ImagesBufferIndex = ImagesBufferIndex%IMAGES_BUFFER_SIZE;
				CameraFramesWatcher[ImagesBufferIndex] = cameraFrameWatcher;
				ImagesBufferIndex++;
			}
		}

		void InitializeCameraFramesWatcher()
		{
			for (int i = 0; i < IMAGES_BUFFER_SIZE; i++)
				CameraFramesWatcher.Add(new CameraFrameWatcher(new Bitmap(100,100), new DateTime()));
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
			IsNowPlaying = true;
			return; //TODO: TEST (Camera isn't working now)
			MjpegCamera.FrameReady += BmpToImageSource;
			MjpegCamera.FrameReady += BmpToCameraFramesWatcher;
			MjpegCamera.ErrorHandler += GetError;
			VideoThread = new Thread(MjpegCamera.StartVideo);
			VideoThread.Start();
		}
		public void StopVideo()
		{
			//TODO: TEST (Camera isn't working now)
			{
				IsNowPlaying = false;
				return;
			}
			MjpegCamera.StopVideo();
			VideoThread.Join(5000);
			MjpegCamera.FrameReady -= BmpToImageSource;
			MjpegCamera.FrameReady -= BmpToCameraFramesWatcher;
			MjpegCamera.ErrorHandler -= GetError;
			ImageSource = new BitmapImage();
			IsNowPlaying = false;
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