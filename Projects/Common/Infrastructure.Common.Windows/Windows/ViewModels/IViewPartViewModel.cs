using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Infrastructure.Common.Windows.ViewModels
{
	public interface IViewPartViewModel : INotifyPropertyChanged
	{
		string Key { get; }
		void OnShow();
		void OnHide();

		void RegisterShortcut(KeyGesture keyGesture, Action command);
		void RegisterShortcut(KeyGesture keyGesture, RelayCommand command);
		void RegisterShortcut<T>(KeyGesture keyGesture, RelayCommand<T> command, Func<T> getArg);
	}
}