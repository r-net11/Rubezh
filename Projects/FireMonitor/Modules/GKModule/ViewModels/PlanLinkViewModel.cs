using System;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace GKModule.ViewModels
{
	public class PlanLinkViewModel : BaseViewModel
	{
		public PlanLinkViewModel(Plan plan, ElementBase elementBase)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			Plan = plan;
			ElementUID = elementBase.UID;
		}

		public Plan Plan { get; set; }
		public Guid ElementUID { get; set; }
		public XDevice Device { get; set; }
		public XZone Zone { get; set; }
		public XGuardZone GuardZone { get; set; }
		public XDirection Direction { get; set; }
		public XDoor Door { get; set; }

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