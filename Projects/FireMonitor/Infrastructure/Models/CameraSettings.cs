using System;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
    [DataContract]
    public class CameraSettings
    {
        [DataMember]
        public Guid Camera { get; set; }
    }
}
