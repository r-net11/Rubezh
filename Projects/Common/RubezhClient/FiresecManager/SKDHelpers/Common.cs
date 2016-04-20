using RubezhAPI;
using Infrastructure.Common.Windows.Windows;

namespace RubezhClient.SKDHelpers
{
	static class Common
	{
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
				if (!operationResult.Error.Contains("При установлении соединения с SQL Server произошла ошибка") &&
					!operationResult.Error.Contains("Could not open a connection to SQL Server") &&
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