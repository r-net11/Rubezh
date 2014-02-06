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
		public Camera Camera { get; set; }
		private const int IMAGES_BUFFER_SIZE = 10;
		private int ImagesBufferIndex = 0;
		private MjpegCamera MjpegCamera { get; set; }
		public List<StringBuilder> ErrorLog { get; private set; }
		public bool HasError { get; private set; }
		public CameraViewModel(Camera camera)
		{
			Camera = camera;
			ErrorLog = new List<StringBuilder>();
			MjpegCamera = new MjpegCamera(camera.Address, camera.Login, camera.Password);
			ImagesBuffer = new List<Bitmap>(IMAGES_BUFFER_SIZE);
			InitializeImagesBuffer();
		}
		public bool IsNowPlayed { get; private set; }
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
		List<Bitmap> ImagesBuffer { get; set; }
		void InitializeImagesBuffer()
		{
			for (int i = 0; i < IMAGES_BUFFER_SIZE; i++)
				ImagesBuffer.Add(new Bitmap(100, 100));
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
			IsNowPlayed = true;
			MjpegCamera.FrameReady += BmpToImageSource;
			MjpegCamera.ErrorHandler += GetError;
			VideoThread = new Thread(MjpegCamera.StartVideo);
			VideoThread.Start();
		}
		public void PauseVideo()
		{

		}
		public void StopVideo()
		{
			MjpegCamera.StopVideo();
			VideoThread.Join(5000);
			MjpegCamera.FrameReady -= BmpToImageSource;
			MjpegCamera.ErrorHandler -= GetError;
			ImageSource = new BitmapImage();
			IsNowPlayed = false;
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