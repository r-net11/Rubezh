using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models.Plans
{
    [DataContract]
    public class ElementEllipse : ElementBase
    {
        [DataMember]
        public byte[] BackgroundPixels { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color BorderColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }
    }
}
