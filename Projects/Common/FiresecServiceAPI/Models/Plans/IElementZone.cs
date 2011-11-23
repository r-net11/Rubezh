using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public interface IElementZone
    {
        [DataMember]
        ulong? ZoneNo { get; set; }

        Zone Zone { get; set; }
    }
}