using System;
using Infrastructure.Common.Windows.ViewModels;
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

		public VideoViewModel(string videoPath)
		{
			VideoPath = videoPath;
			Title = Resources.Language.VideoViewModel.Title;
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