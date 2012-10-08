using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
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
            foreach (var plan in Plans)
            {
                AllPlans.Add(plan);
                AddChild(plan);
            }
        }

        void AddChild(Plan parentPlan)
        {
            foreach (var plan in parentPlan.Children)
            {
                plan.Parent = parentPlan;
                AllPlans.Add(plan);
                AddChild(plan);
            }
        }

		public override bool ValidateVersion()
		{
			bool result = true;
			foreach (var plan in Plans)
			{
				if (plan.ElementXDevices == null)
				{
					plan.ElementXDevices = new List<ElementXDevice>();
					result = false;
				}
			}
			return result;
		}
    }
}