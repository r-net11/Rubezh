using System;
using System.ComponentModel;
using System.Windows.Threading;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    class AsyncInstanceOperationHelper
    {
        ProgressViewModel _progressViewModel;
        Action _work;
        Action _completed;

        public void Run(Action work, Action completed, string tite)
        {
            _work = work;
            _completed = completed;
            _progressViewModel = new ProgressViewModel(tite);

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.RunWorkerAsync();

            ServiceFactory.UserDialogs.ShowModalWindow(_progressViewModel);
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_work != null)
                _work();
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(StopProgress));

            if (_completed != null)
                _completed();
        }

        public void StopProgress()
        {
            _progressViewModel.CloseProgress();
        }
    }
}
