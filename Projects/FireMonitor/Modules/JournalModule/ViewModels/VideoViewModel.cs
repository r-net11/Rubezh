using System;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
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
            var camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUID);
			if (camera != null)
			{
				var rviSettings = ClientManager.SystemConfiguration.RviSettings;
				HasVideo = RviClient.RviClientHelper.GetVideoFile(rviSettings, eventUID, camera, VideoPath, out errorInformation);
				ErrorInformation = errorInformation;
			}
			else
			{
				ErrorInformation = "Не найдена камера";
			}
		}
		public override bool OnClosing(bool isCanceled)
		{
			if (File.Exists(VideoPath))
				File.Delete(VideoPath);
			return base.OnClosing(isCanceled);
		}
	}
}