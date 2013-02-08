using FiresecAPI.Models;

namespace ServerFS2.Helpers
{
    public class ParametersHelper
    {
        public static Property CreateProperty(int paramValue, DriverProperty driverProperty)
        {
            var offsetParamValue = paramValue;

            var highByteValue = paramValue / 256;
            var lowByteValue = paramValue - highByteValue * 256;

            if (driverProperty.HighByte)
                offsetParamValue = highByteValue;
            else if (driverProperty.LargeValue)
                offsetParamValue = paramValue;
            else
                offsetParamValue = lowByteValue;

            if (driverProperty.MinBit > 0)
            {
                byte byteOffsetParamValue = (byte)offsetParamValue;
                byteOffsetParamValue = (byte)(byteOffsetParamValue >> driverProperty.MinBit);
                byteOffsetParamValue = (byte)(byteOffsetParamValue << driverProperty.MinBit);
                offsetParamValue = byteOffsetParamValue;
            }

            if (driverProperty.MaxBit > 0)
            {
                byte byteOffsetParamValue = (byte)offsetParamValue;
                byteOffsetParamValue = (byte)(byteOffsetParamValue << 8 - driverProperty.MaxBit);
                byteOffsetParamValue = (byte)(byteOffsetParamValue >> 8 - driverProperty.MaxBit);
                offsetParamValue = byteOffsetParamValue;
            }

            if (driverProperty.BitOffset > 0)
            {
                offsetParamValue = offsetParamValue >> driverProperty.BitOffset;
            }

            if (driverProperty.Caption == "Задержка включения МРО, сек")
            {
                offsetParamValue = offsetParamValue * 5;
            }

            var property = new Property()
            {
                Name = driverProperty.Name,
                Value = offsetParamValue.ToString()
            };

            return property;
        }
    }
}
