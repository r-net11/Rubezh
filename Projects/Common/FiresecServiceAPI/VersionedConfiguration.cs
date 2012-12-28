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

        public virtual void Initialize()
        {
        }

        public virtual bool ValidateVersion()
        {
			return true;
        }

        [DataMember]
        public ConfigurationVersion Version { get; set; }
    }
}