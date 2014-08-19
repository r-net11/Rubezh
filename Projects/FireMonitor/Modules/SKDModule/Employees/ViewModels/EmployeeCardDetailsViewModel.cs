using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation;
		public SKDCard Card { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }
		public bool IsNewCard { get; private set; }

		public EmployeeCardDetailsViewModel(Organisation organisation, SKDCard card = null)
		{
			ChangeDeactivationControllerCommand = new RelayCommand(OnChangeDeactivationController);
			ChangeReaderCommand = new RelayCommand(OnChangeReader);
			ShowUSBCardReaderCommand = new RelayCommand(OnShowUSBCardReader);

			Organisation = organisation;
			Card = card;
			if (card == null)
			{
				IsNewCard = true;
				Title = "Создание пропуска";
				card = new SKDCard()
				{
					Number = 0,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddYears(1)
				};
			}
			else
			{
				Title = string.Format("Свойства пропуска: {0}", card.Number);
			}
			Card = card;
			Number = Card.Number;
			Password = Card.Password;
			StartDate = Card.StartDate;
			EndDate = Card.EndDate;
			UserTime = Card.UserTime;
			DeactivationControllerUID = Card.DeactivationControllerUID;

			AccessDoorsSelectationViewModel = new AccessDoorsSelectationViewModel(Organisation, Card.CardDoors);

			AvailableAccessTemplates = new ObservableCollection<AccessTemplate>();
			AvailableAccessTemplates.Add(new AccessTemplate() { Name = "<нет>" });
			var accessTemplateFilter = new AccessTemplateFilter();
			accessTemplateFilter.OrganisationUIDs.Add(Organisation.UID);
			var accessTemplates = AccessTemplateHelper.Get(accessTemplateFilter);
			if (accessTemplates != null)
			{
				foreach (var accessTemplate in accessTemplates)
					AvailableAccessTemplates.Add(accessTemplate);
			}

			SelectedAccessTemplate = AvailableAccessTemplates.FirstOrDefault(x => x.UID == Card.AccessTemplateUID);
			StopListCards = new ObservableCollection<SKDCard>();
			var stopListCards = CardHelper.GetStopListCards();
			if (stopListCards == null)
				return;
			foreach (var item in stopListCards)
				StopListCards.Add(item);
			SelectedStopListCard = StopListCards.FirstOrDefault();

			CardTypes = new ObservableCollection<CardType>(Enum.GetValues(typeof(CardType)).OfType<CardType>());
			SelectedCardType = Card.CardType;
		}

		int _number;
		public int Number
		{
			get { return _number; }
			set
			{
				_number = value;
				OnPropertyChanged(() => Number);
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		ObservableCollection<CardType> _cardTypes;
		public ObservableCollection<CardType> CardTypes
		{
			get { return _cardTypes; }
			set
			{
				_cardTypes = value;
				OnPropertyChanged(() => CardTypes);
			}
		}

		CardType _selectedCardType;
		public CardType SelectedCardType
		{
			get { return _selectedCardType; }
			set
			{
				_selectedCardType = value;
				OnPropertyChanged(() => SelectedCardType);
				OnPropertyChanged(() => CanSelectEndDate);
				OnPropertyChanged(() => CanSetUserTime);

				if (value != CardType.Temporary)
				{
					EndDate = StartDate;
				}
			}
		}

		public bool CanSelectEndDate
		{
			get { return SelectedCardType == CardType.Temporary || SelectedCardType == CardType.Duress; }
		}

		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged(() => StartDate);

				if (SelectedCardType != CardType.Temporary)
				{
					EndDate = value;
				}
			}
		}

		DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				OnPropertyChanged(() => EndDate);
			}
		}

		int _userTime;
		public int UserTime
		{
			get { return _userTime; }
			set
			{
				_userTime = value;
				OnPropertyChanged(() => UserTime);
			}
		}

		public bool CanSetUserTime
		{
			get { return SelectedCardType == CardType.OneTime; }
		}

		Guid DeactivationControllerUID;

		ObservableCollection<AccessTemplate> _availableAccessTemplates;
		public ObservableCollection<AccessTemplate> AvailableAccessTemplates
		{
			get { return _availableAccessTemplates; }
			set
			{
				_availableAccessTemplates = value;
				OnPropertyChanged(() => AvailableAccessTemplates);
			}
		}

		AccessTemplate _selectedAccessTemplate;
		public AccessTemplate SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				OnPropertyChanged(() => SelectedAccessTemplate);
			}
		}

		bool _useStopList;
		public bool UseStopList
		{
			get { return _useStopList; }
			set
			{
				_useStopList = value;
				OnPropertyChanged(() => UseStopList);
				UpdateStopListCard();
			}
		}

		public ObservableCollection<SKDCard> StopListCards { get; private set; }

		SKDCard _selectedStopListCard;
		public SKDCard SelectedStopListCard
		{
			get { return _selectedStopListCard; }
			set
			{
				_selectedStopListCard = value;
				OnPropertyChanged(() => SelectedStopListCard);
				UpdateStopListCard();
			}
		}

		void UpdateStopListCard()
		{
			if (UseStopList && SelectedStopListCard != null)
			{
				Number = SelectedStopListCard.Number;
			}
		}

		bool _useReader;
		public bool UseReader
		{
			get { return _useReader; }
			set
			{
				_useReader = value;
				OnPropertyChanged(() => UseReader);
				if (value)
				{
					ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournal);
				}
				else
				{
					ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournal);
				}
			}
		}

		public void OnNewJournal(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.ObjectUID == ClientSettings.SKDSettings.CardCreatorReaderUID)
				{
					if (journalItem.CardNo > 0)
					{
						Number = journalItem.CardNo;
					}
				}
			}
		}

		public RelayCommand ChangeDeactivationControllerCommand { get; private set; }
		void OnChangeDeactivationController()
		{
			var controllerSelectationViewModel = new ControllerSelectationViewModel(DeactivationControllerUID);
			if (DialogService.ShowModalWindow(controllerSelectationViewModel))
			{
				DeactivationControllerUID = controllerSelectationViewModel.SelectedDevice.UID;
				OnPropertyChanged(() => DeactivationControllerName);
			}
		}

		public string DeactivationControllerName
		{
			get
			{
				var controllerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == DeactivationControllerUID);
				if (controllerDevice != null)
				{
					return controllerDevice.Name;
				}
				else
				{
					return "Нажмите для выбора контроллера";
				}
			}
		}

		public RelayCommand ChangeReaderCommand { get; private set; }
		void OnChangeReader()
		{
			var readerSelectationViewModel = new ReaderSelectationViewModel(ClientSettings.SKDSettings.CardCreatorReaderUID);
			if (DialogService.ShowModalWindow(readerSelectationViewModel))
			{
				OnPropertyChanged(() => ReaderName);
			}
		}

		public string ReaderName
		{
			get
			{
				var readerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == ClientSettings.SKDSettings.CardCreatorReaderUID);
				if (readerDevice != null)
				{
					return readerDevice.Name;
				}
				else
				{
					return "Нажмите для выбора считывателя";
				}
			}
		}

		public RelayCommand ShowUSBCardReaderCommand { get; private set; }
		void OnShowUSBCardReader()
		{
			var cardNumberViewModel = new CardNumberViewModel();
			if (DialogService.ShowModalWindow(cardNumberViewModel))
			{
				Number = cardNumberViewModel.Number;
			}
		}

		protected override bool Save()
		{
			if (EndDate < StartDate)
			{
				MessageBoxService.ShowWarning("Дата конца действия пропуска не может быть раньше даты начала действия");
				return false;
			}
			if (SelectedCardType == CardType.Temporary || SelectedCardType == CardType.Duress)
			{
				if (EndDate < DateTime.Now)
				{
					MessageBoxService.ShowWarning("Дата конца действия пропуска не может быть меньше теущей даты");
					return false;
				}
			}
			if(Number <= 0)
			{
				MessageBoxService.ShowWarning("Номер карты должен быто положительным числом");
				return false;
			}

			if (UseStopList && SelectedStopListCard != null)
			{
				Card.UID = SelectedStopListCard.UID;
				Card.IsInStopList = false;
				Card.StopReason = null;
			}
			Card.Number = Number;
			Card.Password = Password;
			Card.CardType = SelectedCardType;
			Card.StartDate = StartDate;
			Card.EndDate = EndDate;
			Card.UserTime = UserTime;
			Card.DeactivationControllerUID = DeactivationControllerUID;
			Card.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();
			Card.CardDoors.ForEach(x => x.CardUID = Card.UID);
			Card.OrganisationUID = Organisation.UID;

			if (SelectedAccessTemplate != null)
				Card.AccessTemplateUID = SelectedAccessTemplate.UID;
			if (AvailableAccessTemplates.IndexOf(SelectedAccessTemplate) == 0)
				Card.AccessTemplateUID = null;
			ServiceFactory.Events.GetEvent<NewCardEvent>().Publish(Card);
			return true;

		}
	}
}