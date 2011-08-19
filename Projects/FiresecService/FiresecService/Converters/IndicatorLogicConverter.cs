using System.Linq;
using Firesec.Indicator;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class IndicatorLogicConverter
    {
        public static IndicatorLogic Convert(LEDProperties lEDProperties)
        {
            var indicatorLogic = new IndicatorLogic();

            switch (lEDProperties.type)
            {
                case "0":
                    indicatorLogic.IndicatorLogicType = IndicatorLogicType.Zone;
                    break;

                case "1":
                    indicatorLogic.IndicatorLogicType = IndicatorLogicType.Device;
                    break;
            }

            if (lEDProperties.zone != null)
            {
                foreach (string zone in lEDProperties.zone)
                {
                    indicatorLogic.Zones.Add(zone);
                }
            }

            if (lEDProperties.device != null && lEDProperties.device.Count() > 0)
            {
                var indicatorDevice = lEDProperties.device[0];
                indicatorLogic.DeviceUID = indicatorDevice.UID;
                indicatorLogic.OnColor = StringToIndicatorColorType(indicatorDevice.state1);
                indicatorLogic.OffColor = StringToIndicatorColorType(indicatorDevice.state2);
                indicatorLogic.FailureColor = StringToIndicatorColorType(indicatorDevice.state3);
                indicatorLogic.ConnectionColor = StringToIndicatorColorType(indicatorDevice.state4);
            }

            return indicatorLogic;
        }

        public static LEDProperties ConvertBack(IndicatorLogic indicatorLogic)
        {
            var lEDProperties = new LEDProperties();

            switch (indicatorLogic.IndicatorLogicType)
            {
                case IndicatorLogicType.Zone:
                    lEDProperties.type = "0";
                    lEDProperties.device = null;
                    lEDProperties.zone = indicatorLogic.Zones.ToArray();
                    break;

                case IndicatorLogicType.Device:
                    lEDProperties.type = "1";
                    lEDProperties.zone = null;
                    lEDProperties.device = new deviceType[1];
                    lEDProperties.device[0] = new deviceType();

                    lEDProperties.device[0].UID = indicatorLogic.DeviceUID;
                    lEDProperties.device[0].state1 = IndicatorColorTypeToString(indicatorLogic.OnColor);
                    lEDProperties.device[0].state2 = IndicatorColorTypeToString(indicatorLogic.OffColor);
                    lEDProperties.device[0].state3 = IndicatorColorTypeToString(indicatorLogic.FailureColor);
                    lEDProperties.device[0].state4 = IndicatorColorTypeToString(indicatorLogic.ConnectionColor);
                    break;
            }

            return lEDProperties;
        }

        public static IndicatorColorType StringToIndicatorColorType(string stringIndicatorColor)
        {
            switch (stringIndicatorColor)
            {
                case "0":
                    return IndicatorColorType.None;

                case "1":
                    return IndicatorColorType.Red;

                case "2":
                    return IndicatorColorType.Green;

                case "3":
                    return IndicatorColorType.Orange;

                case "4":
                    return IndicatorColorType.RedBlink;

                case "5":
                    return IndicatorColorType.GreenBlink;

                case "6":
                    return IndicatorColorType.OrangeBlink;
            }

            return IndicatorColorType.None;
        }

        public static string IndicatorColorTypeToString(IndicatorColorType indicatorColorType)
        {
            switch (indicatorColorType)
            {
                case IndicatorColorType.None:
                    return "0";

                case IndicatorColorType.Red:
                    return "1";

                case IndicatorColorType.Green:
                    return "2";

                case IndicatorColorType.Orange:
                    return "3";

                case IndicatorColorType.RedBlink:
                    return "4";

                case IndicatorColorType.GreenBlink:
                    return "5";

                case IndicatorColorType.OrangeBlink:
                    return "6";
            }

            return "0";
        }
    }
}