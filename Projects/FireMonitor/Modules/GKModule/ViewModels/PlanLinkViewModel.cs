using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Events;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;

namespace GKModule.ViewModels
{
	public class PlanLinkViewModel : BaseViewModel
	{
		public PlanLinkViewModel(Plan plan, Guid elementBaseUID)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			Plan = plan;
			ElementUID = elementBaseUID;
		}

		public Plan Plan { get; set; }
		public Guid ElementUID { get; set; }
		public GKBase GkBaseEntity { get; set; }

		public string Name
		{
			get { return Plan.Caption; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(Plan.UID, ElementUID));
		}
	}
}