using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MuliclientAPI
{
    [DataContract]
    public class WindowSize
    {
        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }
    }
}