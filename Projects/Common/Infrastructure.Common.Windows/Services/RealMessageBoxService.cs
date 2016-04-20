using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Services
{
	public class RealMessageBoxService : IMessageBoxService
	{
		public void Show(string message, string title = null)
		{
			MessageBoxService.Show(message, title);
		}
		public bool ShowQuestion(string message, string title = null)
		{
			return MessageBoxService.ShowQuestion(message, title);
		}
		public bool ShowConfirmation(string message, string title = null)
		{
			return MessageBoxService.ShowConfirmation(message, title);
		}
		public void ShowError(string message, string title = null)
		{
			MessageBoxService.ShowError(message, title);
		}
		public void ShowWarning(string message, string title = null)
		{
			MessageBoxService.ShowWarning(message, title);
		}
		public void ShowException(Exception e, string title = null)
		{
			MessageBoxService.ShowException(e, title);
		}
		public MessageBoxResult ShowExtended(string message, string title = null, bool isModal = true)
		{
			return MessageBoxService.ShowExtended(message, title, isModal);
		}
		public MessageBoxResult ShowQuestionExtended(string message, string title = null)
		{
			return MessageBoxService.ShowQuestionExtended(message, title);
		}
		public MessageBoxResult ShowConfirmationExtended(string message, string title = null)
		{
			return MessageBoxService.ShowConfirmationExtended(message, title);
		}
		public MessageBoxResult ShowErrorExtended(string message, string title = null)
		{
			return MessageBoxService.ShowErrorExtended(message, title);
		}
		public MessageBoxResult ShowWarningExtended(string message, string title = null)
		{
			return MessageBoxService.ShowWarningExtended(message, title);
		}
		public MessageBoxResult ShowExceptionExtended(Exception e, string title = null)
		{
			return MessageBoxService.ShowExceptionExtended(e, title);
		}

		public void SetMessageBoxHandler(Action<MessageBoxViewModel, bool> handler)
		{
			MessageBoxService.SetMessageBoxHandler(handler);
		}
		public void ResetMessageBoxHandler()
		{
			MessageBoxService.ResetMessageBoxHandler();
		}
	}
}