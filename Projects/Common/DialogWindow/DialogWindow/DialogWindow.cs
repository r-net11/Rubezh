using System.Windows;
using DialogBox.ViewModel;
using Infrastructure;
using Infrastructure.Common;

namespace DialogBox
{
    public static class DialogBox
    {
        static DialogBox()
        {
            var resourceService = new ResourceService();
            resourceService.AddResource(new ResourceDescription(typeof(UserDialogViewModel).Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static MessageBoxResult Show(string message)
        {
            return ShowWindow(message, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult ShowQuestion(string message)
        {
            return ShowWindow(message, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        public static MessageBoxResult ShowExclamation(string message)
        {
            return ShowWindow(message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public static MessageBoxResult ShowException(string message)
        {
            return ShowWindow(message, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        static MessageBoxResult ShowWindow(string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            var userDialogViewModel = new UserDialogViewModel(message, messageBoxButton, messageBoxImage);
            UserDialogService.ShowModalWindow(userDialogViewModel);
            return userDialogViewModel.Result;
        }
    }
}