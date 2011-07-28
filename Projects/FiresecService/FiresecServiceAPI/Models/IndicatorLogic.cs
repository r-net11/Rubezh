using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class IndicatorLogic
    {
        public IndicatorLogic()
        {
            Zones = new List<string>();
        }

        [DataMember]
        public IndicatorLogicType IndicatorLogicType { get; set; }

        [DataMember]
        public List<string> Zones { get; set; }

        [DataMember]
        public string DeviceUID { get; set; }

        [DataMember]
        public IndicatorColorType OnColor { get; set; }

        [DataMember]
        public IndicatorColorType OffColor { get; set; }

        [DataMember]
        public IndicatorColorType FailureColor { get; set; }

        [DataMember]
        public IndicatorColorType ConnectionColor { get; set; }
    }

    public enum IndicatorLogicType
    {
        Device,
        Zone
    }

    public enum IndicatorColorType
    {
        None = 0,
        Red = 1,
        Green = 2,
        Orange = 3,
        RedBlink = 4,
        GreenBlink = 5,
        OrangeBlink = 6
    }
}
