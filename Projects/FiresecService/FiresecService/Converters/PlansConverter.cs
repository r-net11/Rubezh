using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class PlansConverter
    {
        public static PlansConfiguration Convert(Firesec.Plans.surfaces innerPlans)
        {
            var plansConfiguration = new PlansConfiguration();
            return plansConfiguration;
        }
    }
}
