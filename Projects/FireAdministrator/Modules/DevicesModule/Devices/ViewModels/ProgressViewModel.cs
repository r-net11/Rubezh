using System;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ProgressViewModel : DialogContent
    {
        public ProgressViewModel()
        {
            FiresecEventSubscriber.OperationProgress += new Action<int, string, int, int>(FiresecEventSubscriber_OperationProgress);
        }

        public void CloseProgress()
        {
            FiresecEventSubscriber.OperationProgress -= new Action<int, string, int, int>(FiresecEventSubscriber_OperationProgress);
            Close(true);
        }

        void FiresecEventSubscriber_OperationProgress(int stage, string comment, int percentComplete, int bytesRW)
        {
            Description = comment;
            Percent = percentComplete;
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
    }
}
