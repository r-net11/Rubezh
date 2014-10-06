using System;
using System.Windows;
using Common;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows
{
	public static class MessageBoxService
	{
		public static void Show2(string message, string title = null)
		{
			Show(message, title);
		}
		public static bool ShowQuestion2(string message, string title = null)
		{
			var result = ShowQuestion(message, title);
			switch (result)
			{
				case MessageBoxResult.Yes:
					return true;
				case MessageBoxResult.No:
					return false;
				default:
					return false;
			}
		}
		public static bool ShowConfirmation2(string message, string title = null)
		{
			return ShowConfirmation(message, title) == MessageBoxResult.Yes;
		}
		public static void ShowError2(string message, string title = null)
		{
			ShowError(message, title);
		}
		public static void ShowWarning2(string message, string title = null)
		{
			ShowWarning(message, title);
		}
		public static void ShowException2(Exception e, string title = null)
		{
			ShowException(e, title);
		}
		public static MessageBoxResult Show(string message, string title = null, bool isModal = true)
		{
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Information, false, isModal);
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
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Warning);
		}
		public static MessageBoxResult ShowException(Exception e, string title = null)
		{
			return ShowWindow(title, e.Message + "\n" + e.StackTrace, MessageBoxButton.OK, MessageBoxImage.Error, true);
		}

		static void Show(MessageBoxViewModel viewModel, bool isModal = true)
		{
			if (isModal)
				DialogService.ShowModalWindow(viewModel);
			else
				DialogService.ShowWindow(viewModel);
		}

		private static MessageBoxResult ShowWindow(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false, bool isModal = true)
		{
			var viewModel = new MessageBoxViewModel(title, message, messageBoxButton, messageBoxImage, isException);
			Show(viewModel, isModal);
			return viewModel.Result;
		}
	}
}