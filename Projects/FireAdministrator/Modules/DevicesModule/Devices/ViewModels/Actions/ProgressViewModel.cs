using System.Threading;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ProgressViewModel : DialogContent
    {
        public ProgressViewModel(string title)
        {
            StopCommand = new RelayCommand(OnStop);
            Title = title;
            FiresecEventSubscriber.OperationProgress += new FiresecEventSubscriber.ProgressDelegate(FiresecEventSubscriber_OperationProgress);
        }

        public void CloseProgress()
        {
            FiresecEventSubscriber.OperationProgress -= new FiresecEventSubscriber.ProgressDelegate(FiresecEventSubscriber_OperationProgress);
            Close(true);
        }

        bool FiresecEventSubscriber_OperationProgress(int stage, string comment, int percentComplete, int bytesRW)
        {
            Description = comment;
            Percent = percentComplete;

            CancelText = "Отменить";
            if (stage == -100)
                CancelText = "Остановить";

            if (stage > 0)
            {
                int stageNo = stage / (256 * 256);
                int stageCount = stage - stageNo * 256 * 256;
            }

            return true;
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
            var thread = new Thread(Stop);
            thread.Start();
        }

        void Stop()
        {
            FiresecManager.StopProgress();
        }
    }
}
