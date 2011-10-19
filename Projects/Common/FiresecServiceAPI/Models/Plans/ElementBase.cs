using System.Runtime.Serialization;
using System.Windows;

namespace FiresecAPI.Models
{
    [DataContract]
    public abstract class ElementBase
    {
        public ElementBase()
        {
            Width = 50;
            Height = 50;
        }

        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Width { get; set; }

        public abstract FrameworkElement Draw();
    }
}
