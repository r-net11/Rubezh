using System.Windows;
using DialogBox.ViewModel;
using Infrastructure;

namespace DialogBox
{
    public static class DialogBox
    {
        public static MessageBoxResult Show(string message)
        {
            var userDialogViewModel = new UserDialogViewModel(message);
            ServiceFactory.UserDialogs.ShowModalWindow(userDialogViewModel);

            return userDialogViewModel.Result;
        }

        public static MessageBoxResult Show(string message, MessageBoxButton button)
        {
            var userDialogViewModel = new UserDialogViewModel(message, button);
            ServiceFactory.UserDialogs.ShowModalWindow(userDialogViewModel);

            return userDialogViewModel.Result;
        }

        public static MessageBoxResult Show(string message, MessageBoxButton button, MessageBoxImage image)
        {
            var userDialogViewModel = new UserDialogViewModel(message, button, image);
            ServiceFactory.UserDialogs.ShowModalWindow(userDialogViewModel);

            return userDialogViewModel.Result;
        }
    }
}