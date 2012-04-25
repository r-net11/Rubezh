using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Converters;
using System;

namespace FiresecService
{
    public class FiresecManager
    {
        public List<Driver> Drivers { get; set; }
        public DeviceConfiguration DeviceConfiguration { get; set; }
        public DeviceConfigurationStates DeviceConfigurationStates { get; set; }
        public LibraryConfiguration LibraryConfiguration { get; set; }
        public SystemConfiguration SystemConfiguration { get; set; }
        public PlansConfiguration PlansConfiguration { get; set; }
        public SecurityConfiguration SecurityConfiguration { get; set; }
        public bool IsConnected { get; private set; }
        public string DriversError { get; private set; }

		public FiresecSerializedClient FiresecSerializedClient;

		public void LoadConfiguration()
		{
			DeviceConfiguration = ConfigurationFileManager.GetDeviceConfiguration();
			SecurityConfiguration = ConfigurationFileManager.GetSecurityConfiguration();
			LibraryConfiguration = ConfigurationFileManager.GetLibraryConfiguration();
			SystemConfiguration = ConfigurationFileManager.GetSystemConfiguration();
			PlansConfiguration = ConfigurationFileManager.GetPlansConfiguration();
		}

        public void ConnectFiresecCOMServer(string login, string password)
        {
			FiresecSerializedClient = new FiresecSerializedClient();

            if (FiresecSerializedClient.Connect(login, password).Result)
            {
                ConvertMetadataFromFiresec();
                SetValidChars();
                Update();
				var deviceStatesConverter = new DeviceStatesConverter(this);
                deviceStatesConverter.Convert();

                var watcher = new Watcher(this);
				watcher.Start(FiresecSerializedClient);

                IsConnected = true;
                return;
            }
            IsConnected = false;
        }

        void ConvertMetadataFromFiresec()
        {
            DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
            Drivers = new List<Driver>();
            foreach (var innerDriver in DriverConverter.Metadata.drv)
            {
                var driver = DriverConverter.Convert(innerDriver);
                if (driver == null)
                {
                    DriversError = "Не удается найти данные для драйвера " + innerDriver.name;
                }
                else
                {
                    if (driver.IsIgnore == false)
                        Drivers.Add(driver);
                }
            }
        }

        public void SetValidChars()
        {
            DriverConverter.Metadata = FiresecSerializedClient.GetMetaData().Result;
            var validCharsBuilder = new StringBuilder(DriverConverter.Metadata.drv.Last().validChars);
            validCharsBuilder.Append('№');
            DeviceConfiguration.ValidChars = validCharsBuilder.ToString();
        }

        public void Update()
        {
            var hasInvalidDriver = false;
            DeviceConfiguration.Update();
            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
                if (device.Driver == null)
                {
                    hasInvalidDriver = true;
                    device.Parent.Children.Remove(device);
                }
            }
            if (hasInvalidDriver)
                DeviceConfiguration.Update();
        }
    }
}