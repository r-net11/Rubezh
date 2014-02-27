using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	static class Common
	{
		public static bool ShowErrorIfExists(OperationResult operationResult)
		{
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(operationResult.Error);
				return false;
			}
			return true;
		}

		public static T ShowErrorIfExists<T>(OperationResult<T> operationResult)
			where T : class
		{
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(operationResult.Error);
				return null;
			}
			return operationResult.Result;
		}
	}
}