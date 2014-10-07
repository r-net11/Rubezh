using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Xml.Serialization;
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

		[XmlIgnore]
		public List<Plan> AllPlans { get; set; }

		public void Update()
		{
			AllPlans = new List<Plan>();
			AddChildren(Plans, null);
		}
		private void AddChildren(List<Plan> plans, Plan parent)
		{
			foreach (var plan in plans)
			{
				plan.Parent = parent;
				var realPlan = plan as Plan;
				if (realPlan != null)
					AllPlans.Add(realPlan);
				if (plan.Children == null)
					plan.Children = new List<Plan>();
				AddChildren(plan.Children, plan);
			}
		}

		public override bool ValidateVersion()
		{
			bool result = true;
			Update();
			result &= ValidateVersion(AllPlans);
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
			bool result = true;
			if (plan.ElementGKDevices == null)
			{
				plan.ElementGKDevices = new List<ElementGKDevice>();
				result = false;
			}
			if (plan.ElementRectangleGKZones == null)
			{
				plan.ElementRectangleGKZones = new List<ElementRectangleGKZone>();
				result = false;
			}
			if (plan.ElementPolygonGKZones == null)
			{
				plan.ElementPolygonGKZones = new List<ElementPolygonGKZone>();
				result = false;
			}
			if (plan.ElementRectangleGKGuardZones == null)
			{
				plan.ElementRectangleGKGuardZones = new List<ElementRectangleGKGuardZone>();
				result = false;
			}
			if (plan.ElementPolygonGKGuardZones == null)
			{
				plan.ElementPolygonGKGuardZones = new List<ElementPolygonGKGuardZone>();
				result = false;
			}
			if (plan.ElementRectangleGKDirections == null)
			{
				plan.ElementRectangleGKDirections = new List<ElementRectangleGKDirection>();
				result = false;
			}
			if (plan.ElementPolygonGKDirections == null)
			{
				plan.ElementPolygonGKDirections = new List<ElementPolygonGKDirection>();
				result = false;
			}
			if (plan.ElementGKDoors == null)
			{
				plan.ElementGKDoors = new List<ElementGKDoor>();
				result = false;
			}
			if (plan.ElementExtensions == null)
			{
				plan.ElementExtensions = new List<ElementBase>();
				result = false;
			}

			if (plan.ElementSKDDevices == null)
			{
				plan.ElementSKDDevices = new List<ElementSKDDevice>();
				result = false;
			}
			if (plan.ElementDoors == null)
			{
				plan.ElementDoors = new List<ElementDoor>();
				result = false;
			}
			if (plan.ElementPolygonSKDZones == null)
			{
				plan.ElementPolygonSKDZones = new List<ElementPolygonSKDZone>();
				result = false;
			}
			if (plan.ElementRectangleSKDZones == null)
			{
				plan.ElementRectangleSKDZones = new List<ElementRectangleSKDZone>();
				result = false;
			}

			return result;
		}
	}
}