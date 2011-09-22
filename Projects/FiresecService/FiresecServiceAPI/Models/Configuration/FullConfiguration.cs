using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class FullConfiguration
    {
        public FullConfiguration()
        {
            DeviceConfiguration = new DeviceConfiguration();
            LibraryConfiguration = new LibraryConfiguration();
            PlansConfiguration = new PlansConfiguration();
            SecurityConfiguration = new SecurityConfiguration();
            SystemConfiguration = new SystemConfiguration();
        }

        [DataMember]
        public DeviceConfiguration DeviceConfiguration { get; set; }

        [DataMember]
        public LibraryConfiguration LibraryConfiguration { get; set; }

        [DataMember]
        public PlansConfiguration PlansConfiguration { get; set; }

        [DataMember]
        public SecurityConfiguration SecurityConfiguration { get; set; }

        [DataMember]
        public SystemConfiguration SystemConfiguration { get; set; }
    }
}