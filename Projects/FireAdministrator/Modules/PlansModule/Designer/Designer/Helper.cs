using System;
using System.Linq;
using System.Windows.Media;
using RubezhAPI.Models;
using RubezhClient;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Collections.Generic;
using System.Threading;
using Infrastructure.Client.Plans;
using Infrastructure;
using Infrastructure.Events;

namespace PlansModule.Designer
{
	public static class Helper
	{
		public const string SubPlanAlias = "SubPlan";
		
		public static void UpgradeBackground(IElementBackground element)
		{
			PainterCache.CacheBrush(element);
		}

		public static Plan GetPlan(IElementSubPlan element)
		{
			return ClientManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == element.PlanUID);
		}
		public static string GetSubPlanTitle(IElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			if (plan == null && element.PlanUID != Guid.Empty)
				SetSubPlan(element, null);
			return plan == null ? "Несвязанная ссылка на план" : plan.Caption;
		}
		public static void SetSubPlan(IElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			SetSubPlan(element, plan);
		}
		public static void SetSubPlan(IElementSubPlan element, Plan plan)
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