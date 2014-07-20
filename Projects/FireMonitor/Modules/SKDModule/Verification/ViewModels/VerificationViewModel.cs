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

namespace SKDModule.ViewModels
{
	public class VerificationViewModel : ViewPartViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }
		public PhotoColumnViewModel PhotoColumnViewModel { get; private set; }

		public VerificationViewModel(LayoutPartSKDVerificationProperties layoutPartSKDVerificationProperties)
		{
			DenyCommand = new RelayCommand(OnDeny);
			AllowCommand = new RelayCommand(OnAllow);
			Device = SKDManager.Devices.FirstOrDefault(x => x.UID == layoutPartSKDVerificationProperties.ReaderDeviceUID);
			VerificationItemViewModel = new VerificationItemViewModel();

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
				if (journalItem.ObjectUID == Device.UID)
				{
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
						var shortEmployee = employees.FirstOrDefault();
						if (shortEmployee != null)
						{
							ShortEmployee = shortEmployee;
							var operationResult = FiresecManager.FiresecService.GetEmployeeDetails(shortEmployee.UID);
							if (!operationResult.HasError)
							{
								var employee = operationResult.Result;
								var photo = employee.Photo;
								PhotoColumnViewModel = new PhotoColumnViewModel(employee.Photo);
							}
						}
					}

					var verificationItemViewModel = new VerificationItemViewModel();
					verificationItemViewModel.EmployeeCardID = journalItem.CardNo.ToString();
					VerificationItemViewModel = verificationItemViewModel;
					OnPropertyChanged("VerificationItemViewModel");
					IsCommandEnabled = true;
				}
			}
		}

		public SKDDevice Device { get; private set; }
		public VerificationItemViewModel VerificationItemViewModel { get; private set; }

		bool _isCommandEnabled;
		public bool IsCommandEnabled
		{
			get { return _isCommandEnabled; }
			set
			{
				_isCommandEnabled = value;
				OnPropertyChanged("IsCommandEnabled");
			}
		}

		public RelayCommand DenyCommand { get; private set; }
		void OnDeny()
		{
			//FiresecManager.FiresecService.SKDDenyReader(Device);
			IsCommandEnabled = false;
		}
		bool CanDeny()
		{
			return IsCommandEnabled;
		}

		public RelayCommand AllowCommand { get; private set; }
		void OnAllow()
		{
			//FiresecManager.FiresecService.SKDAllowReader(Device);
			IsCommandEnabled = false;
		}
		bool CanAllow()
		{
			return IsCommandEnabled;
		}

		BackgroundWorker BackgroundWorker;
		void OnBackgroundWorker()
		{

		}

		int _commandTimer;
		public int CommandTimer
		{
			get { return _commandTimer; }
			set
			{
				_commandTimer = value;
				OnPropertyChanged("CommandTimer");
			}
		}
	}
}