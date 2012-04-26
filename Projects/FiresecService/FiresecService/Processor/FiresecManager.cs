using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Converters;
using System;

namespace FiresecService
{
	public partial class FiresecManager
    {
		public FiresecSerializedClient FiresecSerializedClient { get; private set; }
		public ConfigurationManager ConfigurationManager { get; private set; }
		public DeviceConfigurationStates DeviceConfigurationStates { get; set; }

		public FiresecManager()
		{
			FiresecSerializedClient = new FiresecSerializedClient();
			ConfigurationManager = new ConfigurationManager();
			ConfigurationManager.FiresecSerializedClient = FiresecSerializedClient;
		}

		public void LoadConfiguration()
		{
			ConfigurationManager.DeviceConfiguration = ConfigurationFileManager.GetDeviceConfiguration();
			ConfigurationManager.SecurityConfiguration = ConfigurationFileManager.GetSecurityConfiguration();
			ConfigurationManager.LibraryConfiguration = ConfigurationFileManager.GetLibraryConfiguration();
			ConfigurationManager.SystemConfiguration = ConfigurationFileManager.GetSystemConfiguration();
			ConfigurationManager.PlansConfiguration = ConfigurationFileManager.GetPlansConfiguration();
		}

        public bool ConnectFiresecCOMServer(string login, string password)
        {
            if (FiresecSerializedClient.Connect(login, password).Result)
            {
				ConfigurationManager.ConvertMetadataFromFiresec();
				ConfigurationManager.SetValidChars();
				ConfigurationManager.Update();
				ConvertStates();

                var watcher = new Watcher(this);
                return true;
            }
			return false;
        }

		public void Convert()
		{
			ConfigurationManager.Convert();
		}

		public Firesec.CoreConfiguration.config ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
		{
			ConfigurationManager.ConvertBack(deviceConfiguration, includeSecurity);
			return ConfigurationManager.FiresecConfiguration;
		}
    }
}