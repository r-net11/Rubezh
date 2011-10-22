using System.Windows;

namespace Infrastructure.Common
{
    public interface IDialogContent
    {
        string Title { get; }
        void Close(bool result);
        Window Surface { set; }
        object InternalViewModel { get; }
    }
}