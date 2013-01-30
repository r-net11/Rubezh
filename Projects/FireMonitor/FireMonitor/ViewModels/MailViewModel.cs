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
using Infrastructure.Common;

namespace FireMonitor.ViewModels
{
	public class MailViewModel : BaseViewModel
	{
		Dictionary<Zone, StateType> zoneStates = new Dictionary<Zone, StateType>();

		public MailViewModel()
		{
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Unsubscribe(OnZonesStateChanged);
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZonesStateChanged);

			EnableCommand = new RelayCommand(OnEnable);
			IsMailOn = false;

			OnZonesStateChanged(Guid.Empty);

			foreach (var email in FiresecManager.SystemConfiguration.EmailData.Emails)
			{
				Trace.WriteLine(email.Address + " " + MailHelper.PresentStates(email));
			}
		}

		string message;

		public void OnZonesStateChanged(System.Guid guid)
		{
			if (!IsMailOn)
				return;
			foreach (var zone in FiresecManager.Zones)
			{
				foreach (var email in FiresecManager.SystemConfiguration.EmailData.Emails)
				{
					if (email.Zones.Contains(zone.UID) &&
						email.States.Contains(zone.ZoneState.StateType) &&
						IsStateChanged(zone))
					{
						message = " Изменение состояния зоны " +
							zone.PresentationName +
							" на состояние " +
							zone.ZoneState.StateType.ToDescription();
						MailHelper.Send(FiresecManager.SystemConfiguration.EmailData.EmailSettings, email.Address, message, email.MessageTitle);
						//Trace.WriteLine(email.Address + message);
					}
				}
			}
		}

		private bool IsStateChanged(Zone zone)
		{
			//return true;
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

		bool _isMailOn;
		public bool IsMailOn
		{
			get { return _isMailOn; }
			set
			{
				_isMailOn = value;
				OnPropertyChanged("IsMailOn");
			}
		}

		public RelayCommand EnableCommand { get; private set; }
		void OnEnable()
		{
			if (IsMailOn)
			{
				IsMailOn = false;
			}
			else
			{
				IsMailOn = true;
			}
		}
	}
}