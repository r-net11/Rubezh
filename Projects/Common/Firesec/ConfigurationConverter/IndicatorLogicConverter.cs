using System.Linq;
using Common;
using Firesec.Models.IndicatorsLogic;
using FiresecAPI.Models;

namespace Firesec
{
	public static class IndicatorLogicConverter
	{
        public static IndicatorLogic Convert(DeviceConfiguration deviceConfiguration, LEDProperties lEDProperties)
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
				foreach (var item in lEDProperties.zone)
				{
                    if (string.IsNullOrWhiteSpace(item) == false)
                    {
                        int zoneNo = int.Parse(item);
                        var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
                        if (zone != null)
                        {
                            indicatorLogic.ZoneUIDs.Add(zone.UID);
                        }
                    }
				}
			}

			if (lEDProperties.device != null && lEDProperties.device.Count() > 0)
			{
				var indicatorDevice = lEDProperties.device[0];
				indicatorLogic.DeviceUID = GuidHelper.ToGuid(indicatorDevice.UID);
				indicatorLogic.OnColor = StringToIndicatorColorType(indicatorDevice.state1);
				indicatorLogic.OffColor = StringToIndicatorColorType(indicatorDevice.state2);
				indicatorLogic.FailureColor = StringToIndicatorColorType(indicatorDevice.state3);
				indicatorLogic.ConnectionColor = StringToIndicatorColorType(indicatorDevice.state4);
			}

			return indicatorLogic;
		}

		static IndicatorColorType StringToIndicatorColorType(string sate)
		{
			if (sate != null)
			{
				return (IndicatorColorType)int.Parse(sate);
			}
			return IndicatorColorType.Green;
		}

		public static LEDProperties ConvertBack(IndicatorLogic indicatorLogic)
		{
			var lEDProperties = new LEDProperties();

			switch (indicatorLogic.IndicatorLogicType)
			{
				case IndicatorLogicType.Zone:
					lEDProperties.type = "0";
					lEDProperties.device = null;
					lEDProperties.zone = indicatorLogic.Zones.Select(x => x.No.ToString()).ToArray();
					break;

				case IndicatorLogicType.Device:
					lEDProperties.type = "1";
					lEDProperties.zone = null;
					lEDProperties.device = new deviceType[1];
					var indicatorDevice = new deviceType()
					{
						UID = GuidHelper.ToString(indicatorLogic.DeviceUID),
						state1 = ((int)indicatorLogic.OnColor).ToString(),
						state2 = ((int)indicatorLogic.OffColor).ToString(),
						state3 = ((int)indicatorLogic.FailureColor).ToString(),
						state4 = ((int)indicatorLogic.ConnectionColor).ToString()
					};
					if (indicatorLogic.Device != null && indicatorLogic.Device.Driver.DriverType == DriverType.Indicator)
					{
						lEDProperties.type = "2";
						indicatorDevice.state1 = null;
						indicatorDevice.state2 = null;
						indicatorDevice.state3 = null;
						indicatorDevice.state4 = null;
					}
					lEDProperties.device[0] = indicatorDevice;
					break;
			}

			return lEDProperties;
		}
	}
}