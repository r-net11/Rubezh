using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class PlansConfiguration
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
    }
}