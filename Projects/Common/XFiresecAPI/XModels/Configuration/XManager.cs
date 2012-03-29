namespace XFiresecAPI
{
    public static class XManager
    {
        static XManager()
        {
            DeviceConfiguration = new XDeviceConfiguration();
            DriversConfiguration = new XDriversConfiguration();
        }

        public static XDeviceConfiguration DeviceConfiguration;
        public static XDriversConfiguration DriversConfiguration;
    }
}