using Infrastructure.Common;

namespace Infrastructure
{
    public interface ILayoutService
    {
        void Show(IViewPart model);
        void Close();
        void AddAlarmGroups(IViewPart model);
        void ShowAlarm(IViewPart model);
    }
}