using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace StrazhAPI.Models
{
	public interface IVSSContract
	{
		List<VSSCamera> GetCameras();

		void SetCameras(List<VSSCamera> cameras);

		void StartRecording(Guid CameraUID);

		void StopRecording(Guid CameraUID);
	}

	public class VSSCamera
	{
		public string IPAddress { get; set; }

		public int Port { get; set; }

		public string Name { get; set; }

		public string Login { get; set; }

		public string Password { get; set; }

		public int Bitrate { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }
	}

	public class VSSArchiveFilter
	{
		public Guid CameraUID { get; set; }

		public DateTime StartDateTime { get; set; }

		public DateTime EndDateTime { get; set; }
	}

	public class VSSControl : Control
	{
		public void PlayReal(Guid cameraUID)
		{
			;
		}

		public void StopReal()
		{
			;
		}

		public void PlayArchive(VSSArchiveFilter archiveFilter)
		{
			;
		}

		public void StopArchive()
		{
			;
		}

		public void PauseArchive()
		{
			;
		}

		public void RevArchive()
		{
			;
		}

		public void FFArchive()
		{
			;
		}
	}
}