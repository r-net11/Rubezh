using FiresecAPI.Models;

namespace FiresecAPI
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
                return int.Parse(stringAddress);
            }

            var addresses = stringAddress.Split('.');

            int shleifPart = int.Parse(addresses[0]);
            int addressPart = int.Parse(addresses[1]);
            int intAddress = shleifPart * 256 + addressPart;

            return intAddress;
        }
    }
}