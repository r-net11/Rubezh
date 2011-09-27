using System.ComponentModel;

namespace MultiClient.Services
{
    public interface IViewPart :
        INotifyPropertyChanged
    {
        void OnShow();

        void OnHide();
    }
}
