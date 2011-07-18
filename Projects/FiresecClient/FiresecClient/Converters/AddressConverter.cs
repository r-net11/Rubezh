using System;
using FiresecClient.Models;

namespace FiresecClient.Converters
{
    public static class AddressConverter
    {
        public static string IntToStringAddress(Driver driver, int intAddress)
        {
            if (driver.IsDeviceOnShleif == false)
            {
                return intAddress.ToString();
            }

            int shleifPart = intAddress / 256;
            int addressPart = intAddress % 256;
            string stringAddress = shleifPart.ToString() + "." + addressPart.ToString();

            return stringAddress;
        }

        public static int StringToIntAddress(Driver driver, string stringAddress)
        {
            if (driver.HasAddress == false)
            {
                return 0;
            }
            if (driver.IsDeviceOnShleif == false)
            {
                return Convert.ToInt32(stringAddress);
            }

            var addresses = stringAddress.Split('.');

            int shleifPart = System.Convert.ToInt32(addresses[0]);
            int addressPart = System.Convert.ToInt32(addresses[1]);
            int intAddress = shleifPart * 256 + addressPart;

            return intAddress;
        }
    }
}
