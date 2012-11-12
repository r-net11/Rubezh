using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;

namespace DevicesModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan>
	{
		#region IPlanPresenter<Plan> Members

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			return new List<ElementBase>();
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
		}

		#endregion
	}
}
