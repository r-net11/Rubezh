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
		public ConfigurationConverter ConfigurationConverter { get; private set; }
		public DeviceConfigurationStates DeviceConfigurationStates { get; set; }

		public FiresecManager(FiresecService.Service.FiresecService firesecService)
		{
			FiresecService = firesecService;
			FiresecSerializedClient = new FiresecSerializedClient();
			ConfigurationConverter = new ConfigurationConverter()
			{
				FiresecSerializedClient = FiresecSerializedClient
			};

			ConnectFiresecCOMServer();
		}

		bool ConnectFiresecCOMServer()
		{
			string login = AppSettings.OldFiresecLogin;
			string password = AppSettings.OldFiresecPassword;

			if (FiresecSerializedClient.Connect(login, password).Result)
			{
				ConfigurationConverter.ConvertMetadataFromFiresec();
				ConfigurationConverter.SetValidChars();
				ConfigurationConverter.Update();
				ConvertStates();

				var watcher = new Watcher(this);
				return true;
			}
			return false;
		}

		public void Convert()
		{
			ConfigurationConverter.Convert();
		}

		public Firesec.CoreConfiguration.config ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
		{
			ConfigurationConverter.ConvertBack(deviceConfiguration, includeSecurity);
			return ConfigurationConverter.FiresecConfiguration;
		}

		public void ConvertStates()
		{
			DeviceConfigurationStates = new DeviceConfigurationStates();
			if (ConfigurationCash.DeviceConfiguration.Devices.IsNotNullOrEmpty())
			{
				foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
				{
					var deviceState = new DeviceState()
					{
						UID = device.UID,
						PlaceInTree = device.PlaceInTree,
						Device = device
					};

					foreach (var parameter in device.Driver.Parameters)
						deviceState.Parameters.Add(parameter.Copy());

					DeviceConfigurationStates.DeviceStates.Add(deviceState);
				}
			}

			foreach (var zone in ConfigurationCash.DeviceConfiguration.Zones)
			{
				DeviceConfigurationStates.ZoneStates.Add(new ZoneState() { No = zone.No });
			}
		}
	}
}