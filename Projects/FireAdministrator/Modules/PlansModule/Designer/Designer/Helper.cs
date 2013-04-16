using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Media;

namespace PlansModule.Designer
{
	public static class Helper
	{
		public static Plan GetPlan(ElementSubPlan element)
		{
			return FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == element.PlanUID);
		}
		public static string GetSubPlanTitle(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			if (plan == null && element.PlanUID != Guid.Empty)
				SetSubPlan(element, null);
			return plan == null ? "Несвязанный подплан" : plan.Caption;
		}
		public static void UpgradeBackground(IElementBackground element)
		{
			if (element.BackgroundPixels != null)
			{
				var guid = ServiceFactory.ContentService.AddContent(element.BackgroundPixels);
				element.BackgroundImageSource = guid;
				element.BackgroundPixels = null;
				ServiceFactory.SaveService.PlansChanged = true;
			}
			PainterCache.CacheBrush(element);
		}
		public static void SetSubPlan(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			SetSubPlan(element, plan);
		}
		public static void SetSubPlan(ElementSubPlan element, Plan plan)
		{
			element.PlanUID = plan == null ? Guid.Empty : plan.UID;
			element.Caption = plan == null ? string.Empty : plan.Caption;
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