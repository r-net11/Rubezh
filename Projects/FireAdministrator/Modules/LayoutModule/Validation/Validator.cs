﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Validation;
using FiresecClient;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client.Layout;

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
				yield return new LayoutValidationError(layout, "Макет не должен содержать контейнер отдельно от панели навигации", ValidationErrorLevel.Warning);
			if (!isContentExist && isNavigationExist)
				yield return new LayoutValidationError(layout, "Макет не должен содержать панель навигации отдельно от контейнера", ValidationErrorLevel.Warning);
		}
	}
}
