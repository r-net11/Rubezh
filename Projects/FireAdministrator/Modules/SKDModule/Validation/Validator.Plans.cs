using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Interfaces;
using SKDModule.Plans;

namespace SKDModule.Validation
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
            Errors.AddRange(SKDPlanExtension.Instance.FindDuplicateItems<SKDZone, IElementReference>(plan.ElementPolygonSKDZones, plan.ElementRectangleSKDZones).Select(item => new ZoneValidationError(item, GetErrorMessage("Зона", plan), ValidationErrorLevel.Warning, true, plan.UID)));
            Errors.AddRange(SKDPlanExtension.Instance.FindDuplicateItems<SKDDoor, IElementReference>(plan.ElementDoors).Select(item => new DoorValidationError(item, GetErrorMessage("Точка доступа", plan), ValidationErrorLevel.Warning, true, plan.UID)));
        }
        private string GetErrorMessage(string typeName, Plan plan)
        {
            return string.Format("{0} дублируется на плане {1}", typeName, plan.Caption);
        }
    }
}