using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models.Plans
{
    public class CaptionBox
    {
        [DataMember]
        public int idElementCanvas { get; set; }
        [DataMember]
        public double Left { get; set; }
        [DataMember]
        public double Top { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Color { get; set; }
        [DataMember]
        public string BorderColor { get; set; }
    }
}
