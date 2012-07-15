using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;

namespace ItvIntegration
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(DeviceState deviceState)
		{
			DeviceState = deviceState;
			_stateType = deviceState.StateType;
			FiresecCallbackService.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChangedEvent);
			Name = DeviceState.Device.Driver.ShortName + " - " + DeviceState.Device.DottedAddress;
		}

		void OnDeviceStateChangedEvent(Guid uid)
		{
			if (DeviceState.UID == uid)
			{
				StateType = DeviceState.StateType;
			}
		}

		public DeviceState DeviceState { get; private set; }

		public string Name { get; private set; }

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				OnPropertyChanged("StateType");
			}
		}

		public void ExecuteCommand(string commandName)
		{
			if (DeviceState.Device.Driver.IsIgnore)
			{
				var deviceUIDs = new List<Guid>();
				deviceUIDs.Add(DeviceState.UID);

				switch (commandName)
				{
					case "Dasable":
						ItvManager.AddToIgnoreList(deviceUIDs);
						break;

					case "Enable":
						ItvManager.RemoveFromIgnoreList(deviceUIDs);
						break;
				}
			}

			if (DeviceState.Device.Driver.DriverType == DriverType.Valve)
			{
				switch (commandName)
				{
					case "BoltClose":
					case "BoltStop":
					case "BoltOpen":
					case "BoltAutoOn":
					case "BoltAutoOff":
						ItvManager.ExecuteCommand(DeviceState.UID, commandName);
						break;
				}
			}
		}
	}
}