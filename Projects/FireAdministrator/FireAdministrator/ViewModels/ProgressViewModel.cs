using System;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace FireAdministrator.ViewModels
{
	public class ProgressViewModel : DialogViewModel
	{
		bool ContinueProgress { get; set; }

		public ProgressViewModel(string title)
		{
			Title = title;
			CancelText = "Отменить";
			CloseOnEscape = false;
			AllowClose = false;
			StopCommand = new RelayCommand(OnStop);
			ContinueProgress = true;
			FiresecCallbackService.ProgressEvent -= new Func<int, string, int, int, bool>(Progress);
			FiresecCallbackService.ProgressEvent += new Func<int, string, int, int, bool>(Progress);
		}

		public void CloseProgress()
		{
			FiresecCallbackService.ProgressEvent -= new Func<int, string, int, int, bool>(Progress);
			Close(true);
		}

		public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			ApplicationService.Invoke(() => OnProgress(stage, comment, percentComplete, bytesRW));
			return ContinueProgress;
		}

		void OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			Description = comment;
			Percent = percentComplete;

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
			ContinueProgress = false;
			FiresecManager.FiresecService.CancelProgress();
		}
	}
}