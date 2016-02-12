using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infrustructure.Plans.Elements;

namespace GKWebService.Models.GK
{
	public class PlanLinkViewModel
	{
		public Guid ElementUID { get; set; }

		public Guid PlanUID { get; set; }

		public Guid GkBaseEntityUID { get; set; }

		public string Name { get; set; }

		public PlanLinkViewModel()
		{
			
		}

        public PlanLinkViewModel(RubezhAPI.Models.Plan plan, ElementBase elementBase)
		{
			PlanUID = plan.UID;
			ElementUID = elementBase.UID;
	        Name = plan.Caption;
		}

	}
}