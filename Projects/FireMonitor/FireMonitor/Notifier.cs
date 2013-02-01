using System;
using System.Diagnostics;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Mail;
using Infrastructure.Events;

namespace FireMonitor.ViewModels
{
	public static class Notifier
	{
		public static void Initialize()
		{
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Unsubscribe(OnZonesStateChanged);
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZonesStateChanged);

			OnZonesStateChanged(Guid.Empty);

			foreach (var email in FiresecManager.SystemConfiguration.EmailData.Emails)
			{
				Trace.WriteLine(email.Address + " " + MailHelper.PresentStates(email));
			}
		}

		public static void OnZonesStateChanged(System.Guid guid)
		{
			foreach (var zone in FiresecManager.Zones)
			{
				foreach (var email in FiresecManager.SystemConfiguration.EmailData.Emails)
				{
					if (email.Zones.Contains(zone.UID))
					{
						if (!email.States.Contains(zone.ZoneState.StateType))
							email.IsActivated = false;
						else if (!email.IsActivated)
						{
							Notify(zone, email);
							email.IsActivated = true;
						}
					}
				}
			}
		}

		private static void Notify(Zone zone, Email email)
		{
			string message = " Изменение состояния зоны " +
				zone.PresentationName +
				" на состояние " +
				zone.ZoneState.StateType.ToDescription();
			MailHelper.Send(FiresecManager.SystemConfiguration.EmailData.EmailSettings, email.Address, message, email.MessageTitle);
		}
	}
}