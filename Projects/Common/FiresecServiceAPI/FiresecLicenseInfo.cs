using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class FiresecLicenseInfo
    {
        [DataMember]
		public LicenseMode LicenseMode { get; set; }
        [DataMember]
        public int RemoteWorkplacesCount { get; set; }
        [DataMember]
        public bool Fire { get; set; }
        [DataMember]
        public bool Security { get; set; }
        [DataMember]
        public bool Access { get; set; }
        [DataMember]
        public bool Video { get; set; }
        [DataMember]
        public bool OpcServer { get; set; }
    }

	public enum LicenseMode
	{
		NoLicense,
		HasLicense,
		Demonstration
	}
}