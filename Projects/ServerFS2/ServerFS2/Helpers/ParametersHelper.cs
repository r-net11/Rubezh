using System;
using System.Collections.Generic;
using System.Linq;
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


        public static string SetConfigurationParameters(DriverProperty property, Device device)
        {
            var binProperties = new List<BinProperty>();
            var binProperty = new BinProperty();
            var driverProperties = device.Driver.Properties.FindAll(x => x.No == property.No);
            foreach (var driverProperty in driverProperties)
            {
                var value = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name).Value;
                if (driverProperty != null && driverProperty.IsAUParameter)
                {
                    binProperty = binProperties.FirstOrDefault(x => x.No == driverProperty.No);
                    if (binProperty == null)
                    {
                        binProperty = new BinProperty()
                                          {
                                              No = driverProperty.No
                                          };
                        binProperties.Add(binProperty);
                    }
                    int intValue = 0;
                    if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.EnumType)
                    {
                        var driverPropertyParameterValue =
                            driverProperty.Parameters.FirstOrDefault(x => x.Value == value);
                        if (driverPropertyParameterValue != null)
                        {
                            intValue = int.Parse(driverPropertyParameterValue.Value);
                        }
                    }
                    else if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.BoolType)
                    {
                        intValue = Convert.ToInt32(value);
                    }
                    else
                    {
                        intValue = Convert.ToInt32(value);
                        if (driverProperty.Caption == "Задержка включения МРО, сек")
                        {
                            intValue = (int) Math.Truncate((double) intValue/5);
                        }
                    }

                    if (driverProperty.BitOffset > 0)
                    {
                        intValue = intValue << driverProperty.BitOffset;
                    }

                    if (driverProperty.UseMask)
                    {
                        binProperty.HighByte += intValue;
                        binProperty.LowByte = 0xFF;
                    }
                    else if (driverProperty.HighByte)
                        binProperty.LowByte += intValue;
                    else if (driverProperty.LargeValue)
                    {
                        var HighVal = intValue/256;
                        var LowVal = intValue - HighVal*256;
                        binProperty.LowByte = HighVal;
                        binProperty.HighByte = LowVal;
                    }
                    else
                        binProperty.HighByte += intValue;
                }
            }
            return binProperty.ToString();
        }

        static int ExchengeLowAndHigtBytes(int value)
        {
            return value / 256 + (value - (value / 256) * 256) * 256;
        }

        class BinProperty
        {
            public int No;
            public int LowByte;
            public int HighByte;
            public override string ToString()
            {
                var value = (byte)LowByte + (byte)HighByte * 256;
                return value.ToString();
            }
        }

    }
}
