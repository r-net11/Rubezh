using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace FiresecAPI.Models
{
    [DataContract]
    public class IndicatorLogic
    {
        public IndicatorLogic()
        {
            Zones = new List<string>();
        }

        public Device Device { get; set; }

        [DataMember]
        public IndicatorLogicType IndicatorLogicType { get; set; }

        [DataMember]
        public List<string> Zones { get; set; }

        [DataMember]
        public Guid DeviceUID { get; set; }

        [DataMember]
        public IndicatorColorType OnColor { get; set; }

        [DataMember]
        public IndicatorColorType OffColor { get; set; }

        [DataMember]
        public IndicatorColorType FailureColor { get; set; }

        [DataMember]
        public IndicatorColorType ConnectionColor { get; set; }

        public override string ToString()
        {
            switch (IndicatorLogicType)
            {
                case IndicatorLogicType.Device:
                    {
                        if (DeviceUID != Guid.Empty)
                        {
                            var deviceString = "Устр: ";
                            deviceString += Device.Driver.ShortName;
                            deviceString += Device.DottedAddress;
                            return deviceString;
                        }
                        break;
                    }
                case IndicatorLogicType.Zone:
                    {
                        if (Zones != null)
                        {
                            var zonesString = "Зоны: ";

                            for (int i = 0; i < Zones.Count; i++)
                            {
                                if (i > 0)
                                    zonesString += ",";
                                zonesString += Zones[i];
                            }

                            return zonesString;
                        }
                        break;
                    }
            }
            return "";
        }
    }
}