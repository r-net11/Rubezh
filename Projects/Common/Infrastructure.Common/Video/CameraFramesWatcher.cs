using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
		private readonly int _sizeBefore;
		private readonly int _sizeAfter;
		private readonly int _frameInterval;
		private int _imagesBufferIndex;
		public List<CameraFrame> CameraFrames { get; private set; }
		private readonly object _loker = new object();
		private List<StandbyFrames> StandbyFramesList { get; set; }
		private readonly string _videoArchivePath;
		private DateTime _lastDateTime;

		public CameraFramesWatcher(Camera camera, int sizeBeforeBefore = 10, int sizeAfter = 5, int frameInterval = 1000)
		{
			_videoArchivePath = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "VideoArchive");
			StandbyFramesList = new List<StandbyFrames>();
			_sizeBefore = sizeBeforeBefore;
			_sizeAfter = sizeAfter;
			_frameInterval = frameInterval;
			MjpegCamera = new MjpegCamera(camera);
		}

		private void OnFrameReady(Bitmap bmp)
		{
			var dateTime = DateTime.Now;
			lock (_loker)
			{
				if ((CameraFrames != null) && (CameraFrames.Count == _sizeBefore) && (dateTime - _lastDateTime > TimeSpan.FromMilliseconds(_frameInterval)))
				{
					_lastDateTime = dateTime;
					var newbmp = new Bitmap(bmp);
					var cameraFrame = new CameraFrame(newbmp, dateTime);
					_imagesBufferIndex = _imagesBufferIndex%_sizeBefore;
					CameraFrames[_imagesBufferIndex] = cameraFrame;
					_imagesBufferIndex++;
					if ((StandbyFramesList != null) && (StandbyFramesList.Count > 0))
					{
						StandbyFramesList.RemoveAll(x => x.Index == _sizeAfter);
						foreach (var standbyFrames in StandbyFramesList)
						{
							standbyFrames.Index++;
							SavePicture(bmp, standbyFrames.Guid.ToString(), standbyFrames.Index, true);
						}
					}
				}
			}
		}

		void InitializeCameraFramesWatcher()
		{
			_imagesBufferIndex = 0;
			CameraFrames = new List<CameraFrame>(_sizeBefore);
			for (int i = 0; i < _sizeBefore; i++)
				CameraFrames.Add(new CameraFrame(new Bitmap(100, 100), new DateTime()));
		}

		public void Save(Guid guid)
		{
			var dir = new DirectoryInfo(Path.Combine(_videoArchivePath, guid.ToString()));
			if (Directory.Exists(dir.FullName))
				Directory.Delete(dir.FullName, true);
			dir.Create();
			int i = 0;
			lock (_loker)
			{
				CameraFrames = CameraFrames.OrderBy(x => x.DateTime).ToList();
				foreach (var cameraFrame in CameraFrames)
				{
					if (cameraFrame.DateTime.Ticks == 0)
						continue;
					i++;
					SavePicture(cameraFrame.Bitmap, guid.ToString(), i);
				}
			}
			StandbyFramesList.Add(new StandbyFrames(guid));
		}

		private void SavePicture(Bitmap bmp, string dirName, int index, bool after = false)
		{
			var directoryInfo = new DirectoryInfo(Path.Combine(_videoArchivePath, dirName));
			if (!Directory.Exists(directoryInfo.FullName))
				directoryInfo.Create();
			var dateString = DateTime.Now.ToString("HH-mm-ss-ff", CultureInfo.InvariantCulture);
			var fileName = Path.Combine(directoryInfo.FullName, (after?"after - ":"") + index + " (" + dateString + ") " + ".jpg");
			var bmpToSave = new Bitmap(bmp);
			bmpToSave.Save(fileName, ImageFormat.Jpeg);
			bmpToSave.Dispose();
		}

		public void StartVideo()
		{
			InitializeCameraFramesWatcher();
			StandbyFramesList = new List<StandbyFrames>();
			MjpegCamera.FrameReady += OnFrameReady;
			VideoThread = new Thread(MjpegCamera.StartVideo);
			VideoThread.Start();
		}
		public void StopVideo()
		{
			MjpegCamera.FrameReady -= OnFrameReady;
			MjpegCamera.StopVideo();
		}
	}

	public class StandbyFrames
	{
		public Guid Guid { get; private set; }
		public int Index { get; set; }

		public StandbyFrames(Guid guid)
		{
			Guid = guid;
			Index = 0;
		}
	}
}
