#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace GKWebService.Models.Plan
{
	public class PlanSimpl
	{
		public string Description { get; set; }
		public List<PlanElement.PlanElement> Elements { get; set; }
		public double Height { get; set; }
		public string Name { get; set; }
		public IEnumerable<PlanSimpl> NestedPlans { get; set; }
		public Guid? ParentUid { get; set; }
		public Guid Uid { get; set; }
		public double Width { get; set; }
		public bool IsFolder { get; set; }
	}
}
