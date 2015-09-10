using System;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.IO;

namespace JournalModule.ViewModels
{
	public class VideoViewModel : DialogViewModel
	{
		readonly string DirectoryPath = AppDataFolderHelper.GetTempFolder();
		public string VideoPath { get; private set; }
		public bool HasVideo { get; private set; }
		public string ErrorInformation { get; private set; }

		public VideoViewModel(Guid eventUID, Guid cameraUID)
		{
			VideoPath = AppDataFolderHelper.GetTempFileName() + ".avi";
			Title = "Видеофрагмент, связанный с событием";
			string errorInformation;
			HasVideo = RviClient.RviClientHelper.GetVideoFile(FiresecManager.SystemConfiguration, eventUID, cameraUID, VideoPath, out errorInformation);
			ErrorInformation = errorInformation;
		}
		public override bool OnClosing(bool isCanceled)
		{
			if (File.Exists(VideoPath))
				File.Delete(VideoPath);
			return base.OnClosing(isCanceled);
		}
	}
}