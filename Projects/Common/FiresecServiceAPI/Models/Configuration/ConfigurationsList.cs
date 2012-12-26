using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ConfigurationsList : VersionedConfiguration
    {
        public ConfigurationsList()
        {
            Configurations = new List<Configuration>();
        }
        [DataMember]
        public List<Configuration> Configurations { get; set; }
    }
}