using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public interface IZIndexedElement
    {
        [DataMember]
        int ZIndex { get; set; }
    }
}
