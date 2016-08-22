using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Validation;
using FiresecClient;
using Localization.Layout.Errors;
using StrazhAPI.Models.Layouts;

namespace LayoutModule.Validation
{
	class Validator
	{
		public IEnumerable<IValidationError> Validate()
		{
			FiresecManager.LayoutsConfiguration.Update();
			foreach (var layout in FiresecManager.LayoutsConfiguration.Layouts)
				foreach (var error in ValidateLayout(layout))
					yield return error;
		}

		private IEnumerable<IValidationError> ValidateLayout(Layout layout)
		{
			var isContentExist = layout.GetLayoutPartByType(LayoutPartIdentities.Content) != null;
			var isNavigationExist = layout.GetLayoutPartByType(LayoutPartIdentities.Navigation) != null;
			if (isContentExist && !isNavigationExist)
				yield return new LayoutValidationError(layout, CommonErrors.ValidateLayoutIsContentExist_Error, ValidationErrorLevel.Warning);
			if (!isContentExist && isNavigationExist)
                yield return new LayoutValidationError(layout, CommonErrors.ValidateLayoutIsNavigationExist_Error, ValidationErrorLevel.Warning);

			// Проверяем разрешение присутствия элемента "Верификация" в макете на основе данных лицензии
			if (!ServiceFactory.ConfigurationElementsAvailabilityService.IsLayoutVerificationElementsAvailable &&
				layout.GetLayoutPartByType(LayoutPartIdentities.SKDVerification) != null)
                yield return new LayoutValidationError(layout, CommonErrors.ValidateLayoutIsLayoutVerificationElementsAvailable_Error, ValidationErrorLevel.CannotSave);
		}
	}
}
