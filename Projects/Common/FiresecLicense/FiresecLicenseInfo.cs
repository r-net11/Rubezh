using System.Runtime.Serialization;

namespace FiresecLicense
{
    [DataContract]
    public class FiresecLicenseInfo
    {
        [DataMember]
		public LicenseMode LicenseMode { get; set; }
        [DataMember]
        public int RemoteWorkplacesCount { get; set; }
        [DataMember]
        public bool HasFirefighting { get; set; }
        [DataMember]
        public bool HasGuard { get; set; }
        [DataMember]
        public bool HasSKD { get; set; }
        [DataMember]
        public bool HasVideo { get; set; }
        [DataMember]
        public bool HasOpcServer { get; set; }
    }
}