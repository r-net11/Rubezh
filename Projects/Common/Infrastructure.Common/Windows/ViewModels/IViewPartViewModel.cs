using System.ComponentModel;

namespace Infrastructure.Common.Windows.ViewModels
{
    public interface IViewPartViewModel : INotifyPropertyChanged
    {
		string Key { get; }
        void OnShow();
        void OnHide();
    }
}