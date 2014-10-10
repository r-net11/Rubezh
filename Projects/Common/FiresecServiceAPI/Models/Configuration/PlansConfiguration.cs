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
			return result;
		}
	}
}