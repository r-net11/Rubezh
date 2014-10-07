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
			ValidateCodeNoEquality();
			ValidateGuardPasswordEquality();

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (IsManyGK())
					ValidateCodeDifferentGK(code);
			}
		}

		void ValidateCodeNoEquality()
		{
			var codeNos = new HashSet<int>();
			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (!codeNos.Add(code.No))
					Errors.Add(new CodeValidationError(code, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateGuardPasswordEquality()
		{
			var codeNames = new HashSet<string>();
			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (!codeNames.Add(code.Name))
					Errors.Add(new CodeValidationError(code, "Дублируется название кода", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateCodeDifferentGK(GKCode code)
		{
		}
	}
}