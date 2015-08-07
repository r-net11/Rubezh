using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;
using System.Linq;
using System;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateCodes()
		{
			ValidateCodeNoEquality();
			ValidateCodeNameEquality();
			ValidateCodePasswordEquality();

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (IsManyGK)
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

		void ValidateCodeNameEquality()
		{
			var codeNames = new HashSet<string>();
			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (!codeNames.Add(code.Name))
					Errors.Add(new CodeValidationError(code, "Дублируется название кода", ValidationErrorLevel.CannotWrite));
			}
		}
		void ValidateCodePasswordEquality()
		{

			var codePassowrds = new HashSet<int>();
			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (!codePassowrds.Add(code.Password))
					Errors.Add(new CodeValidationError(code, "Дублируется пароль кода", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateCodeDifferentGK(GKCode code)
		{
		}	
	}
}