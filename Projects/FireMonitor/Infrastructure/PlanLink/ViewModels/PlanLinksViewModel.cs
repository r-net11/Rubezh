using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.PlanLink.ViewModels
{
	public sealed class PlanLinksViewModel
	{
		public List<PlanLinkViewModel> Plans {get; private set;}
		public PlanLinksViewModel(List<Guid> planElementUIDs)
		{
			Plans = new List<PlanLinkViewModel>();
			var allPlans = ShowOnPlanHelper.GetAllPlans(planElementUIDs);
			foreach (var paln in allPlans)
			{
				Plans.Add(new PlanLinkViewModel(paln.Key, paln.Value));
			}
		}
	}
}
