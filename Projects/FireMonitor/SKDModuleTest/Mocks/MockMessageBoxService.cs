using Infrastructure.Common.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKDModuleTest.Mocks
{
	public class MockMessageBoxService : IMessageBoxService
	{
		public bool ShowConfirmationResult { get; set; }

		public void Show(string message, string title = null)
		{
			return;
		}

		public bool ShowQuestion(string message, string title = null)
		{
			return ShowConfirmationResult;
		}

		public bool ShowConfirmation(string message, string title = null)
		{
			throw new NotImplementedException();
		}

		public void ShowError(string message, string title = null)
		{
			throw new NotImplementedException();
		}

		public void ShowWarning(string message, string title = null)
		{
		}

		public void ShowException(Exception e, string title = null)
		{
			throw new NotImplementedException();
		}

		public System.Windows.MessageBoxResult ShowExtended(string message, string title = null, bool isModal = true)
		{
			throw new NotImplementedException();
		}

		public System.Windows.MessageBoxResult ShowQuestionExtended(string message, string title = null)
		{
			throw new NotImplementedException();
		}

		public System.Windows.MessageBoxResult ShowConfirmationExtended(string message, string title = null)
		{
			throw new NotImplementedException();
		}

		public System.Windows.MessageBoxResult ShowErrorExtended(string message, string title = null)
		{
			throw new NotImplementedException();
		}

		public System.Windows.MessageBoxResult ShowWarningExtended(string message, string title = null)
		{
			throw new NotImplementedException();
		}

		public System.Windows.MessageBoxResult ShowExceptionExtended(Exception e, string title = null)
		{
			throw new NotImplementedException();
		}

		public void SetMessageBoxHandler(Action<Infrastructure.Common.Windows.Windows.ViewModels.MessageBoxViewModel, bool> handler)
		{
			throw new NotImplementedException();
		}

		public void ResetMessageBoxHandler()
		{
			throw new NotImplementedException();
		}
	}
}