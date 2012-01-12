using Infrastructure;
using Infrastructure.Common;
using FireMonitor.Views;

namespace FireMonitor
{
    public class LayoutService : ILayoutService
    {
        ShellView Shell
        {
            get { return (ShellView)ServiceFactory.ShellView; }
        }

        public void Show(IViewPart model)
        {
            Replace(model);
        }

        public void Close()
        {
            Replace(null);
        }

        void Replace(IViewPart model)
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
            set { Shell.MainContent = value; }
        }

        public void AddAlarmGroups(IViewPart model)
        {
            Shell.AlarmGroups = model;
        }
    }
}