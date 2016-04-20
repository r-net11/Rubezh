using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Common.Windows.Services
{
	public interface IMessageBoxService
	{
		void Show(string message, string title = null);
		bool ShowQuestion(string message, string title = null);
		bool ShowConfirmation(string message, string title = null);
		void ShowError(string message, string title = null);
		void ShowWarning(string message, string title = null);
		void ShowException(Exception e, string title = null);
		MessageBoxResult ShowExtended(string message, string title = null, bool isModal = true);
		MessageBoxResult ShowQuestionExtended(string message, string title = null);
		MessageBoxResult ShowConfirmationExtended(string message, string title = null);
		MessageBoxResult ShowErrorExtended(string message, string title = null);
		MessageBoxResult ShowWarningExtended(string message, string title = null);
		MessageBoxResult ShowExceptionExtended(Exception e, string title = null);
		void SetMessageBoxHandler(Action<MessageBoxViewModel, bool> handler);
		void ResetMessageBoxHandler();
	}
}