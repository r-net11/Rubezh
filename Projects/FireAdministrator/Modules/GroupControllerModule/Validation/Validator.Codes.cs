using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using System.Linq;
using System;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateCodes()
		{
			ValidateCodePropertiesEquality();

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				ValidateCodeStationOnlyOnOneGK(code);
			}
		}

		/// <summary>
		/// Валидация уникальности номеров, названий и паролей кодов
		/// </summary>
		void ValidateCodePropertiesEquality()
		{
			var nos = new HashSet<int>();
			var names = new HashSet<string>();
			var passwords = new HashSet<int>();

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (!nos.Add(code.No))
					Errors.Add(new CodeValidationError(code, "Дублируется номер", ValidationErrorLevel.CannotWrite));

				if (!names.Add(code.Name))
					Errors.Add(new CodeValidationError(code, "Дублируется название кода", ValidationErrorLevel.CannotWrite));

				if (!passwords.Add(code.Password))
					Errors.Add(new CodeValidationError(code, "Дублируется пароль кода", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Код должен зависеть от объектов, присутствующие на одном и только на одном ГК
		/// </summary>
		/// <param name="code"></param>
		bool ValidateCodeStationOnlyOnOneGK(GKCode code)
		{
			if (code.GkParents.Count == 0)
			{
				Errors.Add(new CodeValidationError(code, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
				return false;
			}

			if (code.GkParents.Count > 1)
			{
				Errors.Add(new CodeValidationError(code, "Код содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}
	}
}