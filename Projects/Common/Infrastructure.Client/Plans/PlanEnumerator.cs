using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;

namespace Infrastructure.Client.Plans
{
	public static class PlanEnumerator
	{
		public static IEnumerable<ElementBase> Enumerate(Plan plan)
		{
			foreach (var elementPrimitives in EnumeratePrimitives(plan))
				yield return elementPrimitives;
			foreach (var elementSubPlan in plan.ElementSubPlans)
				yield return elementSubPlan;
		}

		public static IEnumerable<ElementBase> EnumeratePrimitives(Plan plan)
		{
			return new ElementBase[0]
				.Concat(plan.ElementRectangles)
				.Concat(plan.ElementEllipses)
				.Concat(plan.ElementTextBlocks)
				.Concat(plan.ElementPolygons)
				.Concat(plan.ElementPolylines);
		}
	}
}
