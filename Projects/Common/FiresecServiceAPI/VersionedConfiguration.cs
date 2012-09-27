using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class VersionedConfiguration
    {
        public VersionedConfiguration()
        {
            Version = new ConfigurationVersion();
        }

        public virtual void ValidateVersion()
        {

        }

        [DataMember]
        public ConfigurationVersion Version { get; set; }
    }
}