using System;
using FiresecClient;
using Infrastructure.Common;
using FiresecAPI.Models;
using System.Threading;

namespace DevicesModule.ViewModels
{
    public class ProgressViewModel : DialogContent
    {
        public ProgressViewModel(Device device)
        {
            StopCommand = new RelayCommand(OnStop);
            Title = device.Driver.Name + ". Автопоиск устройств";
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

        public RelayCommand StopCommand { get; private set; }
        void OnStop()
        {
            Thread thread = new Thread(Stop);
            thread.Start();
        }

        void Stop()
        {
            FiresecManager.StopProgress();
        }
    }
}
