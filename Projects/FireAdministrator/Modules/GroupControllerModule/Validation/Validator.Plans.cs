﻿using FiresecAPI.GK;
using FiresecAPI.Models;
using RubezhClient;
using GKModule.Plans;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Interfaces;
using System.Linq;

namespace GKModule.Validation
{
	public partial class Validator
	{
		private void ValidatePlans()
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				ValidatePlanZones(plan);
		}
		private void ValidatePlanZones(Plan plan)
		{
			Errors.AddRange(GKPlanExtension.Instance.FindDuplicateItems<GKZone, IElementReference>(plan.ElementPolygonGKZones, plan.ElementRectangleGKZones).Select(item => new ZoneValidationError(item, GetErrorMessage("Зона", plan), ValidationErrorLevel.Warning, true, plan.UID)));
			Errors.AddRange(GKPlanExtension.Instance.FindDuplicateItems<GKGuardZone, IElementReference>(plan.ElementPolygonGKGuardZones, plan.ElementRectangleGKGuardZones).Select(item => new GuardZoneValidationError(item, GetErrorMessage("Охранная зона", plan), ValidationErrorLevel.Warning, true, plan.UID)));
			Errors.AddRange(GKPlanExtension.Instance.FindDuplicateItems<GKDoor, IElementReference>(plan.ElementGKDoors).Select(item => new DoorValidationError(item, GetErrorMessage("Точка доступа", plan), ValidationErrorLevel.Warning, true, plan.UID)));
		}
		private string GetErrorMessage(string typeName, Plan plan)
		{
			return string.Format("{0} дублируется на плане {1}", typeName, plan.Caption);
		}
	}
}