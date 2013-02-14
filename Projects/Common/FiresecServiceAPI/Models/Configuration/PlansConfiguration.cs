using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class PlansConfiguration : VersionedConfiguration
	{
		public PlansConfiguration()
		{
			Plans = new List<Plan>();
			PlanFolders = new List<PlanFolder>();
		}

		[DataMember]
		public List<Plan> Plans { get; set; }
		[DataMember]
		public List<PlanFolder> PlanFolders { get; set; }

		public List<Plan> AllPlans { get; set; }

		public void Update()
		{
			if (PlanFolders == null)
				PlanFolders = new List<PlanFolder>();
			AllPlans = new List<Plan>();
			foreach (var plan in Plans)
			{
				AllPlans.Add(plan);
				AddChild(plan);
			}
			foreach (var planFolder in PlanFolders)
				AddChild(planFolder, null);
		}

		private void AddChild(Plan parentPlan)
		{
			if (parentPlan.Folders == null)
				parentPlan.Folders = new List<PlanFolder>();
			foreach (var plan in parentPlan.Children)
			{
				plan.Parent = parentPlan;
				AllPlans.Add(plan);
				AddChild(plan);
			}
			foreach (var planFolder in parentPlan.Folders)
				AddChild(planFolder, parentPlan);
		}
		private void AddChild(PlanFolder parentPlanFolder, Plan parentPlan)
		{
			foreach (var planFolder in parentPlanFolder.Folders)
				AddChild(planFolder, parentPlan);
			foreach (var plan in parentPlanFolder.Plans)
			{
				plan.Parent = parentPlan;
				AllPlans.Add(plan);
				AddChild(plan);
			}
		}

		public override bool ValidateVersion()
		{
			bool result = true;
			if (PlanFolders == null)
			{
				PlanFolders = new List<PlanFolder>();
				result = false;
			}
			result &= ValidateVersion(Plans);
			result &= ValidateVersion(PlanFolders);
			return result;
		}
		private bool ValidateVersion(List<Plan> plans)
		{
			bool result = true;
			foreach (var plan in plans)
			{
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
				if (plan.Folders == null)
				{
					plan.Folders = new List<PlanFolder>();
					result = false;
				}
				result &= ValidateVersion(plan);
				result &= ValidateVersion(plan.Children);
				result &= ValidateVersion(plan.Folders);
			}
			return result;
		}
		private bool ValidateVersion(List<PlanFolder> folders)
		{
			bool result = true;
			foreach (var folder in folders)
			{
				result &= ValidateVersion(folder.Plans);
				result &= ValidateVersion(folder.Folders);
			}
			return result;
		}
		private bool ValidateVersion(Plan plan)
		{
			bool result = plan.BackgroundPixels == null;
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