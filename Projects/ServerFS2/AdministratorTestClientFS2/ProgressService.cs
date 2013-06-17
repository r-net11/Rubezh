using System;
using System.ComponentModel;
using System.Windows.Threading;
using AdministratorTestClientFS2.ViewModels;
using Common;
using Infrastructure.Common.Windows;

namespace AdministratorTestClientFS2
{
	public class ProgressService //: IProgressService
	{
		ProgressViewModel _progressViewModel;
		Action _work;
		Action _completed;
		public void Run(Action work, Action completed, string title)
		{
			_work = work;
			_completed = completed;
			_progressViewModel = new ProgressViewModel(title);
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += BackgroundWorkerDoWork;
			backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
			backgroundWorker.RunWorkerAsync();

			DialogService.ShowModalWindow(_progressViewModel);
		}
		void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			if (_work != null)
				_work();
		}
		void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(StopProgress));
			if (_completed != null)
				_completed();
		}
		void StopProgress()
		{
			SafeContext.Execute(() => _progressViewModel.CloseProgress());
		}
	}
}