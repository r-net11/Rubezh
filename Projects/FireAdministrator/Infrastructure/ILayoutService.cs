using Infrastructure.Common;

namespace Infrastructure
{
    public interface ILayoutService
    {
        void Show(IViewPart model);
        void Close();
        void ShowMenu(object model);
        void ShowValidationArea(object model);
    }
}
