using System;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.IO;

namespace JournalModule.ViewModels
{
	public class VideoViewModel : DialogViewModel
	{
		readonly string DirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Firesec2", "Video");
		string VideoPath { get; set; }
		public VideoViewModel(Guid eventUID, Guid cameraUID)
		{
			if (!Directory.Exists(DirectoryPath))
				Directory.CreateDirectory(DirectoryPath);
			VideoPath = Path.Combine(DirectoryPath, Guid.NewGuid().ToString()) + ".avi";
			Title = "Видеофрагмент, связанный с событием";
			RviClient.RviClientHelper.GetVideoFile(FiresecManager.SystemConfiguration, eventUID, cameraUID, VideoPath);
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (File.Exists(VideoPath))
				File.Delete(VideoPath);
			return base.OnClosing(isCanceled);
		}
	}
}