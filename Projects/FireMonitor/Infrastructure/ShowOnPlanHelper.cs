using System;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using System.Collections.Generic;
using RubezhAPI.Models.Layouts;
using Infrustructure.Plans.Interfaces;

namespace Infrastructure
{
	public static class ShowOnPlanHelper
	{
		static Guid _layoutUID;
		public static Guid LayoutUID
		{
			get { return _layoutUID; }
			set
			{
				_layoutUID = value;
				CashPlans = GetPlans();
			}
		}

		public static List<Plan> CashPlans { get; private set; }

		public static List<Plan> GetPlans()
		{
			if(LayoutUID == Guid.Empty)
			{
				CashPlans = new List<Plan>(ClientManager.PlansConfiguration.AllPlans);
				return ClientManager.PlansConfiguration.AllPlans;
			}
			else
			{
				var plans = new List<Guid>();
				var layout = ClientManager.LayoutsConfiguration.Layouts.FirstOrDefault(x => x.UID == LayoutUID);
				if(layout != null)
				{
					foreach(var part in layout.Parts)
					{
						if(part.Properties != null && part.Properties is LayoutPartPlansProperties)
						{
							var layoutPartPlansProperties = part.Properties as LayoutPartPlansProperties;
							foreach(var planUID in layoutPartPlansProperties.Plans)
							{
								if(!plans.Any(x=>x == planUID))
								{
									plans.Add(planUID);
								}
							}
						}
					}
				}
				return ClientManager.PlansConfiguration.AllPlans.Where(x => plans.Any(w => w == x.UID)).ToList();
			}
		}

		public static void ShowObjectOnPlan(Plan plan, Guid objectUID)
		{
			if (plan != null)
			ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, plan.AllElements.First(x=> (x as IElementReference).ItemUID == objectUID).UID));
		}

		public static List<Plan> GetAllPlans(Guid objectUID)
		{
			return CashPlans == null ? GetPlans().Where(x => x.AllElements.Any(y => y is IElementReference && (y as IElementReference).ItemUID == objectUID)).ToList() : CashPlans.Where(x => x.AllElements.Any(y => y is IElementReference && (y as IElementReference).ItemUID == objectUID)).ToList();
		}

		public static Plan GetPlan(Guid deviceUID)
		{
			return GetAllPlans(deviceUID).FirstOrDefault();
		}

		public static void ShowGKSKDZone(GKSKDZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowGKSKDZoneOnPlanEvent>().Publish(zone);
		}

		public static void ShowCamera(Camera camera)
		{
			ServiceFactory.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(camera);
		}
	}
}