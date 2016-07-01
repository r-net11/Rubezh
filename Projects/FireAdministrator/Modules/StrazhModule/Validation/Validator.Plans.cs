using System.Linq;
using Localization.Strazh.Common;
using Localization.Strazh.Errors;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Validation;
using StrazhAPI.Plans.Interfaces;
using StrazhModule.Plans;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		private void ValidatePlans()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				ValidatePlanZones(plan);
		}
		private void ValidatePlanZones(Plan plan)
		{
			Errors.AddRange(SKDPlanExtension.Instance.FindDuplicateItems<SKDZone, IElementReference>(plan.ElementPolygonSKDZones, plan.ElementRectangleSKDZones).Select(item => new ZoneValidationError(item, GetErrorMessage(CommonResources.Zone, plan), ValidationErrorLevel.Warning, true, plan.UID)));
			Errors.AddRange(SKDPlanExtension.Instance.FindDuplicateItems<SKDDoor, IElementReference>(plan.ElementDoors).Select(item => new DoorValidationError(item, GetErrorMessage(CommonResources.Door, plan), ValidationErrorLevel.Warning, true, plan.UID)));
		}
		private string GetErrorMessage(string typeName, Plan plan)
		{
			return string.Format(CommonErrors.ValidatePlans_DublicateError, typeName, plan.Caption);
		}
	}
}