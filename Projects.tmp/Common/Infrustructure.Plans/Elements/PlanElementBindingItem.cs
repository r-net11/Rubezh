using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrustructure.Plans.Elements
{
	public class PlanElementBindingItem
	{
		public string PropertyName { get; set; }
		public Guid GlobalVariableUID { get; set; }
	}
}