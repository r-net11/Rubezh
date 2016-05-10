using StrazhAPI;
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
				if (operationResult.Error.Contains(Resources.Language.FiresecManager.SKDHHelpers.Common.DataTruncatedEN))
                    operationResult.Error = Resources.Language.FiresecManager.SKDHHelpers.Common.DataTruncatedRU;
                if (!operationResult.Error.Contains(Resources.Language.FiresecManager.SKDHHelpers.Common.SQLConnectionFaildRU) &&
                    !operationResult.Error.Contains(Resources.Language.FiresecManager.SKDHHelpers.Common.SQLConnectionFaildEN) &&
					showError)
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
				//if (operationResult.Error.Contains("String or binary data would be truncated"))
				//    operationResult.Error = "Превышен максимальный размер строки";
                if (!operationResult.Error.Contains(Resources.Language.FiresecManager.SKDHHelpers.Common.SQLConnectionFaildRU) &&
                    !operationResult.Error.Contains(Resources.Language.FiresecManager.SKDHHelpers.Common.SQLConnectionFaildEN) &&
					showError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				return default(T);
			}
			return operationResult.Result;
		}
	}
}