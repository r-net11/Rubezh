using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.Journal;
using StrazhAPI.Models.Layouts;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace SKDModule.ViewModels
{
	public class VerificationViewModel : ViewPartViewModel
	{
		Guid DeviceUID;

		public VerificationViewModel(LayoutPartReferenceProperties layoutPartSKDVerificationProperties)
		{
			DeviceUID = layoutPartSKDVerificationProperties.ReferenceUID;
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournal);
		}

		public void OnNewJournal(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				var isForVerification = false;
				if (journalItem.JournalEventNameType == JournalEventNameType.Проход_разрешен || journalItem.JournalEventNameType == JournalEventNameType.Проход_запрещен)
				{
					isForVerification = journalItem.ObjectUID == DeviceUID;
				}
				if (isForVerification)
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