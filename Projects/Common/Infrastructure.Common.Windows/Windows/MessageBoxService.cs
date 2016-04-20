using System;
using System.Windows;
using Common;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Windows
{
	public static class MessageBoxService
	{
		public static void Show(string message, string title = null)
		{
			ShowExtended(message, title);
		}
		public static bool ShowQuestion(string message, string title = null)
		{
			var result = ShowWindow(title, message, MessageBoxButton.YesNo, MessageBoxImage.Question);
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
		public static bool ShowConfirmation(string message, string title = null)
		{
			return ShowConfirmationExtended(message, title) == MessageBoxResult.Yes;
		}
		public static void ShowError(string message, string title = null)
		{
			ShowErrorExtended(message, title);
		}
		public static void ShowWarning(string message, string title = null)
		{
			ShowWarningExtended(message, title);
		}
		public static void ShowException(Exception e, string title = null)
		{
			ShowExceptionExtended(e, title);
		}
		public static MessageBoxResult ShowExtended(string message, string title = null, bool isModal = true)
		{
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Information, false, isModal);
		}
		public static MessageBoxResult ShowQuestionExtended(string message, string title = null)
		{
			return ShowWindow(title, message, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
		}
		public static MessageBoxResult ShowConfirmationExtended(string message, string title = null)
		{
			return ShowWindow(title, message, MessageBoxButton.YesNo, MessageBoxImage.Question);
		}
		public static MessageBoxResult ShowErrorExtended(string message, string title = null)
		{
			Logger.Error(message);
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Error);
		}
		public static MessageBoxResult ShowWarningExtended(string message, string title = null)
		{
			return ShowWindow(title, message, MessageBoxButton.OK, MessageBoxImage.Warning);
		}
		public static MessageBoxResult ShowExceptionExtended(Exception e, string title = null)
		{
			return ShowWindow(title, e.Message + "\n" + e.StackTrace, MessageBoxButton.OK, MessageBoxImage.Error, true);
		}

		public static void SetMessageBoxHandler(Action<MessageBoxViewModel, bool> handler)
		{
			_messageBoxHandler = handler;
		}
		public static void ResetMessageBoxHandler()
		{
			_messageBoxHandler = null;
		}

		private static Action<MessageBoxViewModel, bool> _messageBoxHandler;
		private static MessageBoxResult ShowWindow(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false, bool isModal = true)
		{
			var viewModel = new MessageBoxViewModel(title, message, messageBoxButton, messageBoxImage, isException);
			if (_messageBoxHandler == null)
				Show(viewModel, isModal);
			else
				_messageBoxHandler(viewModel, isModal);
			return viewModel.Result;
		}
		private static void Show(MessageBoxViewModel viewModel, bool isModal = true)
		{
			if (isModal)
				DialogService.ShowModalWindow(viewModel);
			else
				DialogService.ShowWindow(viewModel);
		}
	}
}