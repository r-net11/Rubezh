using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class GuardLevel
    {
        public GuardLevel()
        {
            ZoneLevels = new List<ZoneLevel>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<ZoneLevel> ZoneLevels { get; set; }
    }
}
