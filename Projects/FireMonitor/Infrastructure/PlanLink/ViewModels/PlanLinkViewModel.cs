using Infrastructure.Common;
using Infrustructure.Plans.Events;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.PlanLink.ViewModels
{
	public sealed class PlanLinkViewModel
	{
		public Guid ElementUID { get; private set; }
		public Guid PlanUID { get; private set; }
		public string PlanName { get; private set; }
		public PlanLinkViewModel(Plan plan, Guid elementUID)
		{
			ElementUID = elementUID;
			PlanUID = plan.UID;
			PlanName = plan.Caption;
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan); 
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(PlanUID, ElementUID));
		}
	}
}
