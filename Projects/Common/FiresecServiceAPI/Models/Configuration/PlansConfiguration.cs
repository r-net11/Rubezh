using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Linq;
namespace FiresecAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	public class PlansConfiguration : VersionedConfiguration
	{
		public PlansConfiguration()
		{
			Plans = new List<Plan>();
		}

		[DataMember]
		public List<Plan> Plans { get; set; }

		public List<Plan> AllPlans { get; set; }

		public void Update()
		{
			AllPlans = new List<Plan>();
			AddChildren(Plans, null);
			foreach (var plan in Plans)
			{
				var realPlan = plan as Plan;
				if (realPlan != null)
					AllPlans.Add(realPlan);
			}
		}
		private void AddChildren(List<Plan> plans, Plan parent)
		{
			foreach (var plan in plans)
			{
				plan.Parent = parent;
				var realPlan = plan as Plan;
				if (realPlan != null)
					AllPlans.Add(realPlan);
				AddChildren(plan.Children, plan);
			}
		}

		public override bool ValidateVersion()
		{
			bool result = true;
			result &= ValidateVersion(Plans);
			return result;
		}
		private bool ValidateVersion(List<Plan> folders)
		{
			bool result = true;
			foreach (var plan in folders.OfType<Plan>())
				result &= ValidateVersion(plan);
			return result;
		}
		private bool ValidateVersion(Plan plan)
		{
			bool result = plan.BackgroundPixels == null;
			if (plan.ElementXDevices == null)
			{
				plan.ElementXDevices = new List<ElementXDevice>();
				result = false;
			}
			if (plan.ElementRectangleXZones == null)
			{
				plan.ElementRectangleXZones = new List<ElementRectangleXZone>();
				result = false;
			}
			if (plan.ElementPolygonXZones == null)
			{
				plan.ElementPolygonXZones = new List<ElementPolygonXZone>();
				result = false;
			}
			foreach (var elementSubPlan in plan.ElementSubPlans)
				result &= elementSubPlan.BackgroundPixels == null;
			foreach (var elementRectangle in plan.ElementRectangles)
				result &= elementRectangle.BackgroundPixels == null;
			foreach (var elementEllipse in plan.ElementEllipses)
				result &= elementEllipse.BackgroundPixels == null;
			foreach (var elementTextBlock in plan.ElementTextBlocks)
				result &= elementTextBlock.BackgroundPixels == null;
			foreach (var elementPolygon in plan.ElementPolygons)
				result &= elementPolygon.BackgroundPixels == null;
			foreach (var elementPolyline in plan.ElementPolylines)
				result &= elementPolyline.BackgroundPixels == null;
			return result;
		}
	}
}