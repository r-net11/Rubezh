using System.Windows;

namespace MultiClient.Services
{
    public interface IDialogContent
    {
        object InternalViewModel { get; }
        Window Surface { set; }
        string Title { get; }

        void Close(bool result);
    }
}
