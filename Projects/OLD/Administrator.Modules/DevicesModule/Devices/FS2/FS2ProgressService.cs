//using System;
//using System.ComponentModel;
//using System.Windows.Threading;
//using Common;
//using Infrastructure;
//using Infrastructure.Common.Windows;

//namespace DevicesModule.ViewModels
//{
//	public class FS2ProgressService : IProgressService
//	{
//		FS2ProgressViewModel _fs2ProgressViewModel;
//		Action _work;
//		Action _completed;

//		public void Run(Action work, Action completed, string tite)
//		{
//			_work = work;
//			_completed = completed;
//			_fs2ProgressViewModel = new FS2ProgressViewModel(tite);

//			var backgroundWorker = new BackgroundWorker();
//			backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
//			backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
//			backgroundWorker.RunWorkerAsync();

//			DialogService.ShowModalWindow(_fs2ProgressViewModel);
//		}

//		void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
//		{
//			if (_work != null)
//				_work();
//		}

//		void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//		{
//			Dispatcher.CurrentDispatcher.Invoke(new Action(
//				() =>
//				{
//					StopProgress();
//				}
//				));

//			if (_completed != null)
//				_completed();
//		}

//		void StopProgress()
//		{
//			SafeContext.Execute(() => _fs2ProgressViewModel.CloseProgress());
//		}
//	}
//}