//using System;
//using System.Threading.Tasks;
//using FiresecClient;
//using FS2Api;
//using Infrastructure.Common.Windows;
//using Infrastructure.Common.Windows.Windows;
//using Infrastructure.Common.Windows.Windows.ViewModels;

//namespace DevicesModule.ViewModels
//{
//	public class FS2ProgressViewModel : DialogViewModel
//	{
//		public FS2ProgressViewModel(string title)
//		{
//			Title = title;
//			CancelText = "Отменить";
//			CloseOnEscape = false;
//			AllowClose = false;
//			StopCommand = new RelayCommand(OnStop);
//			FiresecManager.FS2ClientContract.Progress -= new Action<FS2ProgressInfo>(Progress);
//			FiresecManager.FS2ClientContract.Progress += new Action<FS2ProgressInfo>(Progress);
//		}

//		public void CloseProgress()
//		{
//			FiresecManager.FS2ClientContract.Progress -= new Action<FS2ProgressInfo>(Progress);
//			Close(true);
//		}

//		public void Progress(FS2ProgressInfo fs2ProgressInfo)
//		{
//			ApplicationService.Invoke(() => OnProgress(fs2ProgressInfo));
//		}

//		void OnProgress(FS2ProgressInfo fs2ProgressInfo)
//		{
//			Description = fs2ProgressInfo.Comment;
//			if (fs2ProgressInfo.PercentComplete >= 0)
//			{
//				Percent = fs2ProgressInfo.PercentComplete;
//			}

//			if (fs2ProgressInfo.StageCount > 1)
//			{
//				var stageText = fs2ProgressInfo.CurrentStage + " из " + fs2ProgressInfo.StageCount;
//				Description = stageText + "\n" + fs2ProgressInfo.Comment;
//			}
//		}

//		int _percent;
//		public int Percent
//		{
//			get { return _percent; }
//			set
//			{
//				_percent = value;
//				OnPropertyChanged(()=>Percent);
//			}
//		}

//		string _description;
//		public string Description
//		{
//			get { return _description; }
//			set
//			{
//				_description = value;
//				OnPropertyChanged(()=>Description);
//			}
//		}

//		string _cancelText;
//		public string CancelText
//		{
//			get { return _cancelText; }
//			set
//			{
//				_cancelText = value;
//				OnPropertyChanged(()=>CancelText);
//			}
//		}

//		public RelayCommand StopCommand { get; private set; }
//		void OnStop()
//		{
//			Task.Factory.StartNew(() =>
//				{
//					FiresecManager.FS2ClientContract.CancelProgress();
//				});
//		}
//	}
//}