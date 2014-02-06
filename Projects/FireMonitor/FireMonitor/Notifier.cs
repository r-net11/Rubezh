using System;
using System.Linq;
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
		}

		public static void OnZonesStateChanged(System.Guid guid)
		{
			foreach (var email in FiresecManager.SystemConfiguration.EmailData.Emails)
			{
				if (email.IsActivated)
					CheckForDeactivate(email);
				else
					CheckForNotify(email);
			}
		}

		static void CheckForNotify(Email email)
		{
			foreach (var zone in FiresecManager.Zones)
			{
				if (email.Zones.Contains(zone.UID) && 
					email.States.Contains(zone.ZoneState.StateType))
				{
					Notify(zone, email);
					email.IsActivated = true;
					break;
				}
			}
		}

		static void CheckForDeactivate(Email email)
		{
			bool shouldDeactivate = true;
			foreach (var zoneGuid in email.Zones)
			{
				var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneGuid);
				if (email.States.Contains(zone.ZoneState.StateType))
				{
					shouldDeactivate = false;
					break;
				}
			}
			if (shouldDeactivate)
			{
				email.IsActivated = false;
			}
		}

		static void Notify(Zone zone, Email email)
		{
			string message = " Изменение состояния зоны " +
				zone.PresentationName +
				" на состояние " +
				zone.ZoneState.StateType.ToDescription();
			MailHelper.Send(FiresecManager.SystemConfiguration.EmailData.EmailSettings, email.Address, message, email.MessageTitle);
		}
	}
}