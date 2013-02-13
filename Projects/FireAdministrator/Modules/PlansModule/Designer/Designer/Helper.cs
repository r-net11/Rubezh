using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrastructure;

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
	}
}
