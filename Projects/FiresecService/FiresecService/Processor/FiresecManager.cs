using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.Configuration;
using FiresecService.Service;
using System.Linq;
using FiresecService.ViewModels;

namespace FiresecService.Processor
{
	public partial class FiresecManager
	{
		public List<FiresecService.Service.FiresecService> FiresecServices { get; set; }
		public FiresecSerializedClient FiresecSerializedClient { get; private set; }
		public ConfigurationConverter ConfigurationConverter { get; private set; }
		public DeviceConfigurationStates DeviceConfigurationStates { get; set; }
		public Watcher Watcher { get; private set; }
		public bool IsConnectedToComServer { get; private set; }
		public bool MustMonitorStates { get; private set; }

		public FiresecManager(bool mustMonitorStates)
		{
			MustMonitorStates = mustMonitorStates;
			FiresecServices = new List<Service.FiresecService>();
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

			if (IsConnectedToComServer = FiresecSerializedClient.Connect(login, password).Result)
			{
				if (MustMonitorStates)
				{
					UILogger.Log("Конвертирование метаданных");
					ConfigurationConverter.ConvertMetadataFromFiresec();
					UILogger.Log("Обновление конфигурации");
					ConfigurationConverter.Update();
					UILogger.Log("Синхронизация конфигурации");
					ConfigurationConverter.SynchronyzeConfiguration();
					UILogger.Log("Конвертирование состояний");
					ConvertStates();
				}
				Watcher = new Watcher(this);
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
			if (Watcher != null)
			{
				Watcher.DoNotCallback = true;
				Watcher.OnStateChanged();
				Watcher.OnParametersChanged();
				Watcher.DoNotCallback = false;
			}
		}
	}
}