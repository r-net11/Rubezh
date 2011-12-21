using System.Runtime.Serialization;

namespace Infrastructure.Models
{
    [DataContract]
    public class AutoActivationSettings
    {
        [DataMember]
        public bool IsAutoActivation { get; set; }

        [DataMember]
        public bool IsPlansAutoActivation { get; set; }
    }
}