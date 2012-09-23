using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System;
using FiresecAPI;
using System.Diagnostics;

namespace DiagnosticsModule.ViewModels
{
    public class DiagnosticsViewModel : ViewPartViewModel
    {
        public DiagnosticsViewModel()
        {
            ShowImitatorCommand = new RelayCommand(OnShowImitator);
            PollCommand = new RelayCommand(OnPoll);
            StopPollCommand = new RelayCommand(OnStopPoll);
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
			ImitatorService.Show();
        }

        public RelayCommand PollCommand { get; private set; }
        void OnPoll()
        {
            //IAsyncResult res = FiresecManager.FiresecService.BeginPoll(0, DateTime.Now, new AsyncCallback(AddCallbackDC), (IFiresecService)FiresecManager.FiresecService);
        }

        static void AddCallbackDC(IAsyncResult ar)
        {
            try
            {
                IFiresecService res = ar.AsyncState as IFiresecService;
                if (res != null)
                {
                    var result = res.EndPoll(ar);
                    Trace.WriteLine("Result = " + result);
                }
            }
            catch (Exception e)
            {

            }
        }

        public RelayCommand StopPollCommand { get; private set; }
        void OnStopPoll()
        {
            FiresecManager.FiresecService.Test();
        }
    }
}