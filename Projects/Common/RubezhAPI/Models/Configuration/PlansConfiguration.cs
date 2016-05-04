using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
namespace RubezhAPI.Models
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
				{
					AllPlans.Add(realPlan);
				}
				plan.Children = new List<Plan>();
				AddChildren(plan.Children, plan);
			}
		}
	}
}