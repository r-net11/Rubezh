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
            Test1Command = new RelayCommand(OnTest1);
            Test2Command = new RelayCommand(OnTest2);
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
			ImitatorService.Show();
        }

        public RelayCommand Test1Command { get; private set; }
        void OnTest1()
        {
        }

        public RelayCommand Test2Command { get; private set; }
        void OnTest2()
        {
            return;
            FiresecManager.FiresecService.Test("ConfigurationChanged");
        }
    }
}