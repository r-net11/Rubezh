using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Journal;
using RubezhAPI.Models.Layouts;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class VerificationViewModel : ViewPartViewModel
	{
		Guid DeviceUID;

		public VerificationViewModel(LayoutPartReferenceProperties layoutPartSKDVerificationProperties)
		{
			DeviceUID = layoutPartSKDVerificationProperties.ReferenceUID;
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournals);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournals);
		}

		public void OnNewJournals(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.JournalEventNameType == JournalEventNameType.Проход_пользователя_разрешен)
				{
					var door = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (door != null)
					{
						if ((journalItem.JournalEventDescriptionType == JournalEventDescriptionType.Вход_Глобал && door.EnterDeviceUID == DeviceUID) ||
							(journalItem.JournalEventDescriptionType == JournalEventDescriptionType.Выход_Глобал && door.ExitDeviceUID == DeviceUID))
						{
							EventName = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventNameType);
							DateTime = journalItem.SystemDateTime.ToString();

							if (journalItem.EmployeeUID != Guid.Empty)
							{
								var operationResult = ClientManager.FiresecService.GetEmployeeDetails(journalItem.EmployeeUID);
								if (!operationResult.HasError && operationResult.Result != null)
								{
									Employee = operationResult.Result;
									PhotoColumnViewModel = new PhotoColumnViewModel(Employee.Photo);
									Organisation = OrganisationHelper.GetSingle(Employee.OrganisationUID);
									continue;
								}
							}
							Employee = null;
							PhotoColumnViewModel = null;
							Organisation = null;
						}
					}
				}
			}
		}

		string _eventName;
		public string EventName
		{
			get { return _eventName; }
			set
			{
				_eventName = value;
				OnPropertyChanged(() => EventName);
			}
		}

		string _dateTime;
		public string DateTime
		{
			get { return _dateTime; }
			set
			{
				_dateTime = value;
				OnPropertyChanged(() => DateTime);
			}
		}

		Employee _employee;
		public Employee Employee
		{
			get { return _employee; }
			set
			{
				_employee = value;
				OnPropertyChanged(() => Employee);
			}
		}

		PhotoColumnViewModel _photoColumnViewModel;
		public PhotoColumnViewModel PhotoColumnViewModel
		{
			get { return _photoColumnViewModel; }
			set
			{
				_photoColumnViewModel = value;
				OnPropertyChanged(() => PhotoColumnViewModel);
			}
		}

		Organisation _organisation;
		public Organisation Organisation
		{
			get { return _organisation; }
			set
			{
				_organisation = value;
				OnPropertyChanged(() => Organisation);
			}
		}
	}
}