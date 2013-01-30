using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Mail;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace FireMonitor.ViewModels
{
	public class MailViewModel : BaseViewModel
	{
		Dictionary<Zone, StateType> zoneStates = new Dictionary<Zone, StateType>();

		public MailViewModel()
		{
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Unsubscribe(OnZonesStateChanged);
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZonesStateChanged);

			CurrentStateType = StateType.Norm;
			OnZonesStateChanged(Guid.Empty);

			foreach (var email in FiresecManager.SystemConfiguration.Emails)
			{
				Trace.WriteLine(email.Address + " " + MailHelper.PresentStates(email));
			}
		}

		public StateType CurrentStateType { get; private set; }

		string message;

		public void OnZonesStateChanged(System.Guid guid)
		{
			foreach (var zone in FiresecManager.Zones)
			{
				foreach (var email in FiresecManager.SystemConfiguration.Emails)
				{
					if (email.Zones.Contains(zone.UID) &&
						email.States.Contains(zone.ZoneState.StateType) &&
						IsStateChanged(zone))
					{
						message = " Изменение состояния зоны " +
							zone.PresentationName +
							" на состояние " +
							zone.ZoneState.StateType.ToDescription();
						//MailHelper.Send(FiresecManager.SystemConfiguration.SenderParams, email.Address, message, email.MessageTitle);
						Trace.WriteLine(email.Address + message);
					}
				}
			}
		}

		private bool IsStateChanged(Zone zone)
		{
			return true;
			if (!zoneStates.ContainsKey(zone))
			{
				zoneStates.Add(zone, zone.ZoneState.StateType);
				return true;
			}
			KeyValuePair<Zone, StateType> kvp = zoneStates.FirstOrDefault(x => x.Key == zone);
			if (kvp.Value == zone.ZoneState.StateType)
				return false;
			else
			{
				zoneStates.Remove(zone);
				zoneStates.Add(zone, zone.ZoneState.StateType);
				return true;
			}
		}
	}
}