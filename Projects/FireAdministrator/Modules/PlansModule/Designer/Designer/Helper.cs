using System;
using System.Linq;
using Localization.Plans.Common;
using StrazhAPI;
using StrazhAPI.Models;
using FiresecClient;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace PlansModule.Designer
{
	public static class Helper
	{
		public const string SubPlanAlias = "SubPlan";

		public static void UpgradeBackground(IElementBackground element)
		{
			PainterCache.CacheBrush(element);
		}
		
		public static Plan GetPlan(ElementSubPlan element)
		{
			return FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == element.PlanUID);
		}
		public static string GetSubPlanTitle(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			if (plan == null && element.PlanUID != Guid.Empty)
				SetSubPlan(element, null);
			return plan == null ? CommonResources.UnrelatedReference : plan.Caption;
		}
		public static void SetSubPlan(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			SetSubPlan(element, plan);
		}
		public static void SetSubPlan(ElementSubPlan element, Plan plan)
		{
			element.PlanUID = (plan == null) ? Guid.Empty : plan.UID;
			element.Caption = (plan == null) ? string.Empty : plan.Caption;
			element.BackgroundColor = GetSubPlanColor(plan);
		}
		public static Color GetSubPlanColor(Plan plan)
		{
			Color color = Colors.Black;
			if (plan != null)
				color = Colors.Green;
			return color;
		}
	}
}