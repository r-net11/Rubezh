using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateCodes()
		{
			ValidateGuardPasswordEquality();

			foreach (var code in XManager.DeviceConfiguration.Codes)
			{
				if (IsManyGK())
					ValidateCodeDifferentGK(code);
			}
		}

		void ValidateGuardPasswordEquality()
		{
			var codeNames = new HashSet<string>();
			foreach (var code in XManager.DeviceConfiguration.Codes)
			{
				if (!codeNames.Add(code.Name))
					Errors.Add(new CodeValidationError(code, "Дублируется название кода", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateCodeDifferentGK(XCode code)
		{
		}
	}
}