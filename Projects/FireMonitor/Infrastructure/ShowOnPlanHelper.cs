using Infrastructure.Events;
using Infrustructure.Plans.Events;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
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
				return CashPlans = ClientManager.PlansConfiguration.AllPlans.Where(x => !x.IsNotShowPlan).ToList();
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
							if (layoutPartPlansProperties.Type == LayoutPartPlansType.All)
							{
								return CashPlans = ClientManager.PlansConfiguration.AllPlans.Where(x=> !x.IsNotShowPlan).ToList();
							}
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

		public static bool ShowObjectOnPlan(IPlanPresentable planElement)
		{
			if (planElement.PlanElementUIDs.Any())
			{
				var plan = GetAllPlans(planElement).FirstOrDefault();
				if (plan.Key != null)
				{
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.Key.UID, plan.Value));
					return false;
				}
				return true;
			}
			return true;
		}

		public static Dictionary<Plan, Guid> GetAllPlans(IPlanPresentable planElement)
		{
		    Dictionary<Plan, Guid> planDictinary = new Dictionary<Plan, Guid>();
			var plans = CashPlans == null ? GetPlans() : CashPlans;
			plans.ForEach(x =>
			{
				var element = x.AllElements.FirstOrDefault(y => planElement.PlanElementUIDs.Contains(y.UID));
				if (element != null)
					planDictinary.Add(x, element.UID);
			});

			return planDictinary;
		}

		public static bool CanShowOnPlan(IPlanPresentable planElement)
		{
			return planElement.PlanElementUIDs.Any() &&  GetAllPlans(planElement).Any();
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