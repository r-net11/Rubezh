using FiresecAPI;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	static class Common
	{
		public static bool ShowErrorIfExists(OperationResult operationResult, bool showError = true)
		{
			if (operationResult == null)
			{
				return false;
			}
			if (operationResult.HasError)
			{
				if (showError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				return false;
			}
			return true;
		}

		public static T ShowErrorIfExists<T>(OperationResult<T> operationResult, bool showError = true)
		{
			if (operationResult == null)
			{
				return default(T);
			}
			if (operationResult.HasError)
			{
				if (showError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				return default(T);
			}
			return operationResult.Result;
		}
	}
}