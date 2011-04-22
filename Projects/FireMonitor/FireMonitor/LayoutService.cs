using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;

namespace FireMonitor
{
    public class LayoutService : ILayoutService
    {
        private ShellView _shellView;

        private ShellView Shell
        {
            get { return _shellView ?? (_shellView = ServiceFactory.Get<ShellView>()); }
        }

        public void Show(IViewPart model)
        {
            Replace(model);
        }

        public void Close()
        {
            Replace(null);
        }

        private void Replace(IViewPart model)
        {
            if (ActiveViewModel != null)
            {
                if (ActiveViewModel is StaticRegionViewModel)
                {
                    //
                }
                else
                {
                    IViewPart temp = ActiveViewModel;
                    try
                    {
                        //temp.Dispose();
                    }
                    catch (Exception ex)
                    {
                        //Logger.Error("Error in LayoutService::Replace", ex);
                    }
                }
            }
            ActiveViewModel = model;
        }

        IViewPart ActiveViewModel
        {
            get { return Shell.MainContent; }
            set
            {
                Shell.MainContent = value;
            }
        }

        public void AddAlarmGroups(IViewPart model)
        {
            Shell.AlarmGroups = model;
        }

        public void ShowAlarm(IViewPart model)
        {
            Shell.Alarm = model;
        }
    }
}
