using System;
using System.ServiceModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.IO;

namespace JournalModule.ViewModels
{
	public class VideoViewModel : DialogViewModel
	{
		public string VideoPath { get; private set; }

		public event EventHandler Play;
		public event EventHandler Stop;

		protected virtual void OnPlay()
		{
			if (Play == null) return;
			Play(this, EventArgs.Empty);
		}

		protected virtual void OnStop()
		{
			if (Stop == null) return;
			Stop(this, EventArgs.Empty);
		}

		public VideoViewModel(Guid eventUID, Guid cameraUID)
		{
			VideoPath = AppDataFolderHelper.GetTempFileName() + ".mkv";
			Title = "Видеофрагмент, связанный с событием";
			try
			{
				RviClient.RviClientHelper.GetVideoFile(FiresecManager.SystemConfiguration, eventUID, cameraUID, VideoPath);
			}
			catch (CommunicationObjectFaultedException e)
			{
				Logger.Error(e, "Исключение при вызове VideoViewModel(Guid eventUID, Guid cameraUID)");
				MessageBoxService.ShowError("Проверьте запущено ли приложение RVi Оператор и настройки соединения с ним");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове VideoViewModel(Guid eventUID, Guid cameraUID)");
			}
		}

		public override bool OnClosing(bool isCanceled)
		{
			OnStop();
			if (File.Exists(VideoPath))
				File.Delete(VideoPath);
			return base.OnClosing(isCanceled);
		}

		public override void Loaded()
		{
			base.Loaded();
			OnPlay();
		}
	}
}