using System;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace StrazhModule.ViewModels
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
		public SKDDevice Device { get; set; }
		public SKDZone Zone { get; set; }
		public SKDDoor Door { get; set; }

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