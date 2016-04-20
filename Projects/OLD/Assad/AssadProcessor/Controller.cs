using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AssadProcessor.Devices;
using FiresecAPI.Models;
using FiresecClient;
using Firesec;
using Infrastructure.Common.Windows;

namespace AssadProcessor
{
	public class Controller
	{
		internal static Controller Current { get; private set; }
		Watcher Watcher;

		public Controller()
		{
			Current = this;
		}

		public void Start()
		{
			FiresecManager.Connect(ClientType.Assad, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.Login, GlobalSettingsHelper.GlobalSettings.Password);
			FiresecManager.GetConfiguration("Assad/Configuration");
			FiresecManager.InitializeFiresecDriver(true);
			FiresecManager.FiresecDriver.Synchronyze();
			FiresecManager.FiresecDriver.StartWatcher(true, false);

			Services.NetManager.Start();
			Watcher = new Watcher();
			Watcher.Start();
		}

		internal void AssadConfig(Assad.MHconfigTypeDevice innerDevice, bool all)
		{
			if (all)
			{
				Services.DeviceManager.Config(innerDevice);
			}
		}

		public Assad.DeviceType[] QueryState(Assad.MHqueryStateType content)
		{
			var assadBase = Configuration.BaseDevices.FirstOrDefault(a => a.DeviceId == content.deviceId);
			if (assadBase != null)
			{
				List<AssadBase> devices = assadBase.FindAllChildren();

				var deviceItems = new List<Assad.DeviceType>();
				foreach (var childAssadBase in devices)
				{
					deviceItems.Add(childAssadBase.GetStates());
				}
				return deviceItems.ToArray();
			}
			return null;
		}

		public Assad.DeviceType QueryAbility(Assad.MHqueryAbilityType content)
		{
			var device = Configuration.BaseDevices.First(a => a.DeviceId == content.deviceId);
			Assad.DeviceType ability = device.QueryAbility();
			return ability;
		}

		public void AssadExecuteCommand(Assad.MHdeviceControlType controlType)
		{
			var assadDevice = Configuration.Devices.First(x => x.DeviceId == controlType.deviceId);
			string commandName = controlType.cmdId;
			if (commandName == "Обновить")
			{
				var cPqueryConfigurationType = new Assad.CPqueryConfigurationType();
				NetManager.Send(cPqueryConfigurationType, null);
			}
			else
			{
				var device = FiresecManager.Devices.FirstOrDefault(x => x.PathId == assadDevice.Id);
				if (device != null)
				{
					if (commandName.StartsWith("Сброс "))
					{
						commandName = commandName.Replace("Сброс ", "");

						if (device.Driver.DriverType == DriverType.Computer)
						{
							foreach (var resetDevice in FiresecManager.Devices)
							{
								if (resetDevice.Driver.States.Any(x => ((x.Name == commandName) && (x.IsManualReset))))
								{
									var resetItem = new ResetItem();
									resetItem.DeviceState = resetDevice.DeviceState;
									var deviceDriverState = resetDevice.DeviceState.States.FirstOrDefault(x => x.DriverState.Name == commandName);
									if (deviceDriverState != null)
									{
										resetItem.States = new List<DeviceDriverState>() { deviceDriverState };
										FiresecManager.FiresecDriver.ResetStates(new List<ResetItem>() { resetItem });
									}
								}
							}
						}
						else
						{
							var resetItem = new ResetItem();
							FiresecManager.FiresecDriver.ResetStates(new List<ResetItem>() { resetItem });
							var deviceDriverState = device.DeviceState.States.FirstOrDefault(x => x.DriverState.Name == commandName);
							if (deviceDriverState != null)
							{
								resetItem.States = new List<DeviceDriverState>() { deviceDriverState };
								FiresecManager.FiresecDriver.ResetStates(new List<ResetItem>() { resetItem });
							}
						}
					}
				}
			}
		}

		public void ResetAllStates(string deviceId)
		{
			var assadDevice = Configuration.Devices.First(x => x.DeviceId == deviceId);
			var device = FiresecManager.Devices.FirstOrDefault(x => x.PathId == assadDevice.Id);
			if (device != null)
			{
				foreach (var state in device.Driver.States)
				{
					if (state.IsManualReset)
					{
						var resetItem = new ResetItem();
						resetItem.DeviceState = device.DeviceState;
						var deviceDriverState = device.DeviceState.States.FirstOrDefault(x => x.DriverState.Name == state.Name);
						if (deviceDriverState != null)
						{
							resetItem.States = new List<DeviceDriverState>() { deviceDriverState };
							FiresecManager.FiresecDriver.ResetStates(new List<ResetItem>() { resetItem });
						}
					}
				}
			}
		}

		public void Stop()
		{
			FiresecManager.Disconnect();
			Services.NetManager.Stop();
		}

		internal bool Ready { get; set; }
	}
}