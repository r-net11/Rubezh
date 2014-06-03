using System;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class PasswordHelper
	{
		public static bool Save(Password Password)
		{
			var operationResult = FiresecManager.FiresecService.SavePassword(Password);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedPassword(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<Password> Get(PasswordFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetPasswords(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
