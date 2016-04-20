using RubezhAPI.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.PlanLink.ViewModels
{
	public sealed class PlanLinksViewModel
	{
		public List<PlanLinkViewModel> Plans {get; private set;}
		public PlanLinksViewModel(IPlanPresentable planElement)
		{
			Plans = new List<PlanLinkViewModel>();
			if (planElement.PlanElementUIDs.Any())
			{
				var allPlans = ShowOnPlanHelper.GetAllPlans(planElement);
				foreach (var paln in allPlans)
				{
					Plans.Add(new PlanLinkViewModel(paln.Key, paln.Value));
				}
			}
		}
	}
}
