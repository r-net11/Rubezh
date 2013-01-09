using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using FiresecAPI.XModels;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System.Threading;
using System.Windows.Threading;
using GKModule.ViewModels;

namespace GKModule
{
    public partial class Watcher
    {
        void StartProgress(string name, int count)
        {
            ApplicationService.Invoke(() =>
            {
                LoadingService.ShowProgress("", name, count);
            });
        }

        void DoProgress(string name)
        {
            ApplicationService.Invoke(() =>
            {
                LoadingService.DoStep(name);
            });
        }

        void StopProgress()
        {
            ApplicationService.Invoke(() =>
            {
                LoadingService.Close();
            });
        }
    }
}