using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;

namespace Infrastructure.Client.Plans
{
	public static class PlanEnumerator
	{
		public static IEnumerable<ElementBase> Enumerate(Plan plan)
		{
			foreach(var elementPrimitives in EnumeratePrimitives(plan))
				yield return elementPrimitives;
			foreach (var elementSubPlan in plan.ElementSubPlans)
				yield return elementSubPlan;
		}
		public static IEnumerable<ElementBase> EnumeratePrimitives(Plan plan)
		{
			foreach (var elementRectangle in plan.ElementRectangles)
				yield return elementRectangle;
			foreach (var elementEllipse in plan.ElementEllipses)
				yield return elementEllipse;
			foreach (var elementTextBlock in plan.ElementTextBlocks)
				yield return elementTextBlock;
			foreach (var elementPolygon in plan.ElementPolygons)
				yield return elementPolygon;
			foreach (var elementPolyline in plan.ElementPolylines)
				yield return elementPolyline;
		}
	}
}
