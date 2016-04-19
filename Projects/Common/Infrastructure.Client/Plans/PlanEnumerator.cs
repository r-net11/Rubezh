using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Client.Plans
{
	public static class PlanEnumerator
	{
		public static IEnumerable<ElementBase> Enumerate(Plan plan)
		{
			return EnumeratePrimitives(plan)
				.Union(plan.ElementSubPlans)
				.Union(plan.ElementPolygonSubPlans);
		}

		public static IEnumerable<ElementBase> EnumeratePrimitives(Plan plan)
		{
			return new ElementBase[0]
				.Concat(plan.ElementRectangles)
				.Concat(plan.ElementEllipses)
				.Concat(plan.ElementTextBlocks)
				.Concat(plan.ElementTextBoxes)
				.Concat(plan.ElementPolygons)
				.Concat(plan.ElementPolylines);
		}
	}
}