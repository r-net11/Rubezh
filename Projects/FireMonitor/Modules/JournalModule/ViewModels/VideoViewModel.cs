using System;
using System.Threading.Tasks;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.IO;
using Localization.Journal.ViewModels;

namespace JournalModule.ViewModels
{
	public class VideoViewModel : DialogViewModel
	{
	    private bool _hasGetVideoAction;

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

		public VideoViewModel(string videoPath, Action getVideoAction = null)
		{
			VideoPath = videoPath;
			Title = CommonViewModels.Videoclip;
		    
            if (getVideoAction != null)
            {
                _hasGetVideoAction = true;
                Task.Factory.StartNew(() =>
                {
                    getVideoAction();
                    ApplicationService.Invoke(OnPlay);
                });
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

		    if (!_hasGetVideoAction)
		    {
                OnPlay();
            }
		}
	}
}