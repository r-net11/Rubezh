using System.ComponentModel;

namespace Infrastructure.Common
{
    public interface IViewPart :
        INotifyPropertyChanged
    {
        void OnShow();
        void OnHide();
    }
}
