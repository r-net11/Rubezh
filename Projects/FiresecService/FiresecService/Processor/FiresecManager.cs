using FiresecAPI.Models;
using FiresecService.Configuration;
using FiresecService.Service;
using System;

namespace FiresecService.Processor
{
	public partial class FiresecManager
	{
		public FiresecService.Service.FiresecService FiresecService { get; set; }
		public FiresecSerializedClient FiresecSerializedClient { get; private set; }
		public ConfigurationConverter ConfigurationManager { get; private set; }
		public DeviceConfigurationStates DeviceConfigurationStates { get; set; }

		public FiresecManager(FiresecService.Service.FiresecService firesecService)
		{
			FiresecSerializedClient = new FiresecSerializedClient();
			FiresecService = firesecService;
			ConfigurationManager = new ConfigurationConverter()
			{
				FiresecSerializedClient = FiresecSerializedClient
			};
		}

		public void LoadConfiguration()
		{
			ConfigurationCash.DeviceConfiguration = ConfigurationFileManager.GetDeviceConfiguration();
			ConfigurationCash.SecurityConfiguration = ConfigurationFileManager.GetSecurityConfiguration();
			ConfigurationCash.LibraryConfiguration = ConfigurationFileManager.GetLibraryConfiguration();
			ConfigurationCash.SystemConfiguration = ConfigurationFileManager.GetSystemConfiguration();
			ConfigurationCash.PlansConfiguration = ConfigurationFileManager.GetPlansConfiguration();
		}

		public bool ConnectFiresecCOMServer()
		{
			string login = AppSettings.OldFiresecLogin;
			string password = AppSettings.OldFiresecPassword;

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