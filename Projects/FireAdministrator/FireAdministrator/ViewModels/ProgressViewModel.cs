using System;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace FireAdministrator.ViewModels
{
	public class ProgressViewModel : DialogViewModel
	{
        bool IsCanceling = false;

		public ProgressViewModel(string title)
		{
			Title = title;
			CancelText = "Отменить";
			CloseOnEscape = false;
			AllowClose = false;
			StopCommand = new RelayCommand(OnStop);
            FiresecManager.FiresecDriver.Watcher.Progress -= new Func<int, string, int, int, bool>(Progress);
            FiresecManager.FiresecDriver.Watcher.Progress += new Func<int, string, int, int, bool>(Progress);
		}

		public void CloseProgress()
		{
            FiresecManager.FiresecDriver.Watcher.Progress -= new Func<int, string, int, int, bool>(Progress);
			Close(true);
		}

		public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
            ApplicationService.Invoke(() => OnProgress(stage, comment, percentComplete, bytesRW));
            return !IsCanceling;
		}

		void OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			Description = comment;
			if (percentComplete >= 0)
			{
				Percent = percentComplete;
			}

			if (stage == -100)
				CancelText = "Остановить";

			if (stage > 0)
			{
				int stageNo = stage / (256 * 256);
				int stageCount = stage - stageNo * 256 * 256;
			}
		}

		int _percent;
		public int Percent
		{
			get { return _percent; }
			set
			{
				_percent = value;
				OnPropertyChanged("Percent");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		string _cancelText;
		public string CancelText
		{
			get { return _cancelText; }
			set
			{
				_cancelText = value;
				OnPropertyChanged("CancelText");
			}
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
            IsCanceling = true;
			//FiresecManager.FiresecDriver.CancelProgress();
		}
	}
}