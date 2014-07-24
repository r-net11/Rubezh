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

		ShortEmployee _shortEmployee;
		public ShortEmployee ShortEmployee
		{
			get { return _shortEmployee; }
			set
			{
				_shortEmployee = value;
				OnPropertyChanged(() => ShortEmployee);
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

		public VerificationViewModel(LayoutPartSKDVerificationProperties layoutPartSKDVerificationProperties)
		{
			Device = SKDManager.Devices.FirstOrDefault(x => x.UID == layoutPartSKDVerificationProperties.ReaderDeviceUID);

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
				if (journalItem.ObjectUID == Device.UID && journalItem.CardNo > 0 &&
					(journalItem.JournalEventNameType == JournalEventNameType.Проход_разрешен || journalItem.JournalEventNameType == JournalEventNameType.Проход_запрещен))
				{
					EventName = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventNameType);
					DateTime = journalItem.SystemDateTime.ToString();

					var cardFilter = new CardFilter()
					{
						FirstNos = journalItem.CardNo,
						LastNos = journalItem.CardNo
					};
					var cards = CardHelper.Get(cardFilter);
					var card = cards.FirstOrDefault();
					if (card != null)
					{
						var employeeFilter = new EmployeeFilter()
						{
							CardNo = journalItem.CardNo
						};
						var employees = EmployeeHelper.Get(employeeFilter);
						ShortEmployee = employees.FirstOrDefault();
						if (ShortEmployee != null)
						{
							var operationResult = FiresecManager.FiresecService.GetEmployeeDetails(ShortEmployee.UID);
							if (!operationResult.HasError)
							{
								var employee = operationResult.Result;
								var photo = employee.Photo;
								PhotoColumnViewModel = new PhotoColumnViewModel(employee.Photo);
								continue;
							}
						}
					}
					ShortEmployee = null;
					PhotoColumnViewModel = null;
				}
			}
		}
	}
}