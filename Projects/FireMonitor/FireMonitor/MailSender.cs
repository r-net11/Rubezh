using System;
using System.Windows;
using Infrastructure;
using Infrastructure.Events;

namespace FireMonitor
{
	public class MailSender
	{
		public MailSender()
		{
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnDevicesStateChanged);

			OnDevicesStateChanged(Guid.Empty);
		}

		public void OnDevicesStateChanged(object obj)
		{
			MessageBox.Show("Event occured");
		}
	}
}