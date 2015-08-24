using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
    public class PlanSimpl
    {
        public string Name { get; set; }

        public Guid Uid { get; set; }

        public string Description { get; set; }

        public List<PlanElement> Elements { get; set; }
        
        public List<PlanSimpl> NestedPlans { get; set; }
    }
}