using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GkWeb.Models.Plan
{
	public class PlanInfo
	{
		public string Description { get; set; }
		public List<PlanElement> Elements { get; set; }
		public double Height { get; set; }
		public string Name { get; set; }
		public IEnumerable<PlanInfo> NestedPlans { get; set; }
		public Guid? ParentUid { get; set; }
		public Guid Uid { get; set; }
		public double Width { get; set; }
		public bool IsFolder { get; set; }
	}
}
