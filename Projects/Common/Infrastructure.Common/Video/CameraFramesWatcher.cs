using System;
using System.Collections.Generic;
using System.Drawing;
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
		public CameraFramesWatcher (Camera camera)
		{
			CameraFrames = new List<CameraFrame>(IMAGES_BUFFER_SIZE);
			MjpegCamera = new MjpegCamera(camera);
			InitializeCameraFramesWatcher();
		}

		private void BmpToCameraFramesWatcher(Bitmap bmp)
		{
			var dateTime = DateTime.Now;
			if (dateTime - CameraFrames.Max().DateTime > TimeSpan.FromSeconds(1))
			{
				var cameraFrame = new CameraFrame(bmp, dateTime);
				ImagesBufferIndex = ImagesBufferIndex % IMAGES_BUFFER_SIZE;
				CameraFrames[ImagesBufferIndex] = cameraFrame;
				ImagesBufferIndex++;
			}
		}

		void InitializeCameraFramesWatcher()
		{
			for (int i = 0; i < IMAGES_BUFFER_SIZE; i++)
				CameraFrames.Add(new CameraFrame(new Bitmap(100, 100), new DateTime()));
		}

		public void Save()
		{
			var dir = new DirectoryInfo("Pictures\\" + DateTime.Now.ToLongDateString());
			if (Directory.Exists(dir.FullName))
				Directory.Delete(dir.FullName);
			foreach (var cameraFrame in CameraFrames)
			{
				cameraFrame.Bitmap.Save(cameraFrame.DateTime.ToLongDateString());
			}
		}

		public void StartVideo()
		{
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
