namespace AssadProcessor
{
    public static class Services
    {
        static Services()
        {
            NetManager = new NetManager();
            DeviceModelManager = new DeviceModelManager();
            DeviceManager = new DeviceManager();
        }

        public static NetManager NetManager { get; private set; }
        public static DeviceModelManager DeviceModelManager { get; private set; }
        public static DeviceManager DeviceManager { get; private set; }
    }
}