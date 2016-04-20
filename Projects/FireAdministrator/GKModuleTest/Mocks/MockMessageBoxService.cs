using Infrastructure.Common.Services;
using NUnit.Framework;
using System;
using System.Windows;

namespace GKModuleTest
{
	public class MockMessageBoxService : IMessageBoxService
	{
		public bool ShowConfirmationResult { get; set; }

		public void Show(string message, string title = null)
		{
		}

		public bool ShowQuestion(string message, string title = null)
		{
			return ShowConfirmationResult;
		}

		public bool ShowConfirmation(string message, string title = null)
		{
			return true;
		}

		public void ShowError(string message, string title = null)
		{
		}

		public void ShowWarning(string message, string title = null)
		{
		}

		public void ShowException(Exception e, string title = null)
		{
			StringAssert.DoesNotMatch("При выполнении операции возникло исключение", title);
		}

		public MessageBoxResult ShowExtended(string message, string title = null, bool isModal = true)
		{
			return MessageBoxResult.OK;
		}

		public MessageBoxResult ShowQuestionExtended(string message, string title = null)
		{
			return MessageBoxResult.OK;
		}

		public MessageBoxResult ShowConfirmationExtended(string message, string title = null)
		{
			return MessageBoxResult.OK;
		}

		public MessageBoxResult ShowErrorExtended(string message, string title = null)
		{
			return MessageBoxResult.OK;
		}

		public MessageBoxResult ShowWarningExtended(string message, string title = null)
		{
			return MessageBoxResult.OK;
		}

		public MessageBoxResult ShowExceptionExtended(Exception e, string title = null)
		{
			return MessageBoxResult.OK;
		}

		public void SetMessageBoxHandler(Action<Infrastructure.Common.Windows.ViewModels.MessageBoxViewModel, bool> handler)
		{
		}

		public void ResetMessageBoxHandler()
		{
		}
	}
}