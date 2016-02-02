using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Models;
using Infrustructure.Plans.Elements;

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