using System.Linq;
using Common;
using Firesec.IndicatorsLogic;
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
                indicatorLogic.Zones.AddRange(lEDProperties.zone.Cast<ulong?>());
            }

            if (lEDProperties.device != null && lEDProperties.device.Count() > 0)
            {
                var indicatorDevice = lEDProperties.device[0];
                indicatorLogic.DeviceUID = GuidHelper.ToGuid(indicatorDevice.UID);
                indicatorLogic.OnColor = (IndicatorColorType) int.Parse(indicatorDevice.state1);
                indicatorLogic.OffColor = (IndicatorColorType) int.Parse(indicatorDevice.state2);
                indicatorLogic.FailureColor = (IndicatorColorType) int.Parse(indicatorDevice.state3);
                indicatorLogic.ConnectionColor = (IndicatorColorType) int.Parse(indicatorDevice.state4);
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
                    lEDProperties.zone = indicatorLogic.Zones.Cast<string>().ToArray();
                    break;

                case IndicatorLogicType.Device:
                    lEDProperties.type = "1";
                    lEDProperties.zone = null;
                    lEDProperties.device = new deviceType[1];
                    lEDProperties.device[0] = new deviceType();

                    lEDProperties.device[0].UID = GuidHelper.ToString(indicatorLogic.DeviceUID);
                    lEDProperties.device[0].state1 = ((int) indicatorLogic.OnColor).ToString();
                    lEDProperties.device[0].state2 = ((int) indicatorLogic.OffColor).ToString();
                    lEDProperties.device[0].state3 = ((int) indicatorLogic.FailureColor).ToString();
                    lEDProperties.device[0].state4 = ((int) indicatorLogic.ConnectionColor).ToString();
                    break;
            }

            return lEDProperties;
        }
    }
}