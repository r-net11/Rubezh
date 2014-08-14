using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Journal;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using FiresecClient.SKDHelpers;
using FiresecClient;
using System;

namespace SKDModule.ViewModels
{
	public class VerificationViewModel : ViewPartViewModel
	{
		public SKDDevice Device { get; private set; }

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

		public VerificationViewModel(LayoutPartReferenceProperties layoutPartSKDVerificationProperties)
		{
			Device = SKDManager.Devices.FirstOrDefault(x => x.UID == layoutPartSKDVerificationProperties.ReferenceUID);

			if (Device != null)
			{
				ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournal);
				ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournal);
			}
		}

		public void OnNewJournal(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.ObjectUID == Device.UID &&
					(journalItem.JournalEventNameType == JournalEventNameType.Проход_разрешен || journalItem.JournalEventNameType == JournalEventNameType.Проход_запрещен))
				{
					EventName = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventNameType);
					DateTime = journalItem.SystemDateTime.ToString();

					if (journalItem.EmployeeUID != Guid.Empty)
					{
						var operationResult = FiresecManager.FiresecService.GetEmployeeDetails(journalItem.EmployeeUID);
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