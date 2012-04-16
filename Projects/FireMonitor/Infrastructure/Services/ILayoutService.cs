using Infrastructure.Common;

namespace Infrastructure
{
    public interface ILayoutService
    {
        void Show(IViewPart viewModel);
        void PreLoad(IViewPart viewModel);
        void Close();
        void AddAlarmGroups(IViewPart viewModel);
    }
}