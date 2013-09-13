using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;
using FiresecAPI.Models;
using XFiresecAPI;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Elements;

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
		public XDirection Direction { get; set; }

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