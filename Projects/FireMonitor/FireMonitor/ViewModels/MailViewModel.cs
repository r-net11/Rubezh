using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Microsoft.Windows.Controls;
using System.Diagnostics;
using Infrastructure.Common.Mail;

namespace FireMonitor.ViewModels
{
	public class MailViewModel : BaseViewModel
	{
		Dictionary<Device, StateType> deviceStates = new Dictionary<Device, StateType>();

		public MailViewModel()
		{
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnDevicesStateChanged);

			CurrentStateType = StateType.Norm;
			OnDevicesStateChanged(Guid.Empty);

			foreach (var email in FiresecManager.SystemConfiguration.Emails)
			{
				Trace.WriteLine(email.Address + " " + MailHelper.PresentStates(email));
			}
		}

		public StateType CurrentStateType { get; private set; }

		public void OnDevicesStateChanged(object obj)
		{
			//var minState = (StateType)Math.Min((int)GetMinASStateType(), (int)GetMinGKStateType());

			//if (CurrentStateType != minState)
			//    CurrentStateType = minState;

			foreach (var device in FiresecManager.Devices)
			{

				if (StateChanged(device))
				{
					foreach (var email in FiresecManager.SystemConfiguration.Emails)
					{
						if (email.SendingStates.Contains(device.DeviceState.StateType))
						{
							//MailHelper.Send(email.Address, "Сообщение отправлено событием класса " + device.DeviceState.StateType.ToDescription() + " прибором " + device.PresentationAddressAndName, "Тест Firesec");
							Trace.WriteLine(email.Address + " Сообщение отправлено событием класса " + device.DeviceState.StateType.ToDescription() + " прибором " + device.PresentationAddressAndName);
						}
					}
				}
			}
		}

		bool StateChanged(Device device)
		{
			if (!deviceStates.ContainsKey(device))
			{
				deviceStates.Add(device, device.DeviceState.StateType);
				return true;
			}
			KeyValuePair<Device, StateType> kvp = deviceStates.FirstOrDefault(x => x.Key == device);
			if (kvp.Value == device.DeviceState.StateType)
				return false;
			else
			{
				deviceStates.Remove(device);
				deviceStates.Add(device, device.DeviceState.StateType);
				return true;
			}
		}

		StateType GetMinASStateType()
		{
			var minStateType = StateType.Norm;
			foreach (var device in FiresecManager.Devices)
				if (device.DeviceState.StateType < minStateType)
					minStateType = device.DeviceState.StateType;
			return minStateType;
		}

		StateType GetMinGKStateType()
		{
			var minStateType = StateType.Norm;
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.DeviceState != null)
				{
					var stateType = device.DeviceState.GetStateType();
					if (stateType < minStateType)
						minStateType = stateType;
				}
			}
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.ZoneState != null)
				{
					var stateType = zone.ZoneState.GetStateType();
					if (stateType < minStateType)
						minStateType = stateType;
				}
			}
			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				if (direction.DirectionState != null)
				{
					var stateType = direction.DirectionState.GetStateType();
					if (stateType < minStateType)
						minStateType = stateType;
				}
			}
			return minStateType;
		}
	}
}