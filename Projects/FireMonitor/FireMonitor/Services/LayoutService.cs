using Infrastructure;
using Infrastructure.Common;

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
                IViewPart temp = ActiveViewModel;
                try
                {
                    //temp.Dispose();
                }
                catch
                {
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