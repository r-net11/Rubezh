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
    }
}
