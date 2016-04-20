using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Validation;
using RubezhClient;
using RubezhAPI.Models.Layouts;
using Infrastructure.Client.Layout;

namespace LayoutModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			ClientManager.LayoutsConfiguration.Update();
			Errors = new List<IValidationError>();
			foreach (var layout in ClientManager.LayoutsConfiguration.Layouts)
				Errors.AddRange(ValidateLayout(layout));
			ValidateLicense();
			return Errors;
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
