using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrustructure.Plans;

namespace DevicesModule.Plans
{
	class PlanExtension : IPlanExtension<Plan>
	{
		#region IPlanExtension<Plan> Members

		public int Index
		{
			get { throw new NotImplementedException(); }
		}

		public string Alias
		{
			get { throw new NotImplementedException(); }
		}

		public string Title
		{
			get { throw new NotImplementedException(); }
		}

		public object TabPage
		{
			get { throw new NotImplementedException(); }
		}

		public bool ElementAdded(Plan plan, Infrustructure.Plans.Elements.ElementBase element)
		{
			throw new NotImplementedException();
		}

		public bool ElementRemoved(Plan plan, Infrustructure.Plans.Elements.ElementBase element)
		{
			throw new NotImplementedException();
		}

		public void RegisterDesignerItem(Infrustructure.Plans.Designer.DesignerItem designerItem)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Infrustructure.Plans.Elements.ElementBase> LoadPlan(Plan plan)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
