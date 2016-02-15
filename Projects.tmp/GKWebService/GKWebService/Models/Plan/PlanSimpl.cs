#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace GKWebService.Models.Plan
{
	public class PlanSimpl
	{
		public string Description { get; set; }
		public List<PlanElement> Elements { get; set; }
		public double Height { get; set; }
		public string Name { get; set; }
		public List<PlanSimpl> NestedPlans { get; set; }
		public Guid Uid { get; set; }
		public double Width { get; set; }
	}
}
