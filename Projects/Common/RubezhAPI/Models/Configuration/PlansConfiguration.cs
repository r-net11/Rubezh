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
			AddToAllPlans(Plans);
		}
		private void AddToAllPlans(List<Plan> plans)
		{
			foreach (var plan in plans)
			{
				AllPlans.Add(plan);
				AddToAllPlans(plan.Children);
			}
		}
	}
}