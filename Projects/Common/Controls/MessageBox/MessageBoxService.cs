using System;
using System.Windows;
using Infrastructure.Common;
using Common;

namespace Controls.MessageBox
{
	public static class MessageBoxService
	{
		static MessageBoxService()
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(MessageBoxViewModel).Assembly, "MessageBox/DataTemplates/Dictionary.xaml"));
		}

		public static MessageBoxResult Show(string message, string title = null)
		{
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public static MessageBoxResult ShowDeviceError(string header, string error)
		{
			return ShowWindow(header, error, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public static MessageBoxResult ShowQuestion(string message, string title = null)
		{
			return ShowWindow(title, message, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
		}

		public static MessageBoxResult ShowConfirmation(string message, string title = null)
		{
			return ShowWindow(title, message, MessageBoxButton.YesNo, MessageBoxImage.Question);
		}

		public static MessageBoxResult ShowError(string message, string title = null)
		{
			Logger.Error(message);
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public static MessageBoxResult ShowWarning(string message, string title = null)
		{
			Logger.Warn(message);
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static MessageBoxResult ShowException(Exception e, string title = null)
		{
			Logger.Error(e);
			string message = e.Message.ToString();
			var stackTraces = e.StackTrace.Split('\n');
			if (stackTraces.Length > 0)
				message += "\n" + stackTraces[0];
			if (stackTraces.Length > 1)
				message += "\n" + stackTraces[1];
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Error, true);
		}

		static MessageBoxResult ShowWindow(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false)
		{
			var messageBoxViewModel = new MessageBoxViewModel(title, message, messageBoxButton, messageBoxImage, isException);
			UserDialogService.ShowModalWindow(messageBoxViewModel);
			return messageBoxViewModel.Result;
		}
	}
}