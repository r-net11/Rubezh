using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;

namespace Infrastructure.Common.Video
{
	public class CameraFramesWatcher
	{
		Thread VideoThread { get; set; }
		private MjpegCamera MjpegCamera { get; set; }
		private const int IMAGES_BUFFER_SIZE = 10;
		private int ImagesBufferIndex;
		public List<CameraFrame> CameraFrames { get; private set; }
		private readonly Object _loker;
		public CameraFramesWatcher(Camera camera)
		{
			MjpegCamera = new MjpegCamera(camera);
			_loker = new object();
			InitializeCameraFramesWatcher();
		}
		private void BmpToCameraFramesWatcher(Bitmap bmp)
		{
			var dateTime = DateTime.Now;
			lock (_loker)
			{
				if ((CameraFrames != null) && (CameraFrames.Count != 0) &&
					(dateTime - CameraFrames.Max().DateTime > TimeSpan.FromSeconds(1)))
				{
					var newbmp = new Bitmap(bmp);
					var cameraFrame = new CameraFrame(newbmp, dateTime);
					ImagesBufferIndex = ImagesBufferIndex % IMAGES_BUFFER_SIZE;
					CameraFrames[ImagesBufferIndex] = cameraFrame;
					ImagesBufferIndex++;
				}
			}
		}

		void InitializeCameraFramesWatcher()
		{
			ImagesBufferIndex = 0;
			CameraFrames = new List<CameraFrame>(IMAGES_BUFFER_SIZE);
			for (int i = 0; i < IMAGES_BUFFER_SIZE; i++)
				CameraFrames.Add(new CameraFrame(new Bitmap(100, 100), new DateTime()));
		}

		public void Save()
		{
			var dir = new DirectoryInfo("Pictures");
			if (Directory.Exists(dir.FullName))
				Directory.Delete(dir.FullName, true);
			dir.Create();
			int i = 1;
			lock (_loker)
			{
				foreach (var cameraFrame in CameraFrames)
				{
					string fileName = dir.FullName + "\\" + i + ".jpg";
					if (cameraFrame.DateTime.Ticks == 0)
						return;
					var bmp = new Bitmap(cameraFrame.Bitmap);
					bmp.Save(fileName, ImageFormat.Jpeg);
					bmp.Dispose();
					i++;
				}
			}
		}

		public void StartVideo()
		{
			InitializeCameraFramesWatcher();
			MjpegCamera.FrameReady += BmpToCameraFramesWatcher;
			VideoThread = new Thread(MjpegCamera.StartVideo);
			VideoThread.Start();
		}
		public void StopVideo()
		{
			MjpegCamera.FrameReady -= BmpToCameraFramesWatcher;
			MjpegCamera.StopVideo();
		}
	}
}