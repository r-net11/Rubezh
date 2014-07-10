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

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation;
		public SKDCard Card { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }
		bool IsNewCard;

		public EmployeeCardDetailsViewModel(Organisation organisation, SKDCard card = null)
		{
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
			StartDate = Card.StartDate;
			EndDate = Card.EndDate;

			AccessDoorsSelectationViewModel = new AccessDoorsSelectationViewModel(Organisation, Card.CardDoors, Card.UID);

			AvailableAccessTemplates = new ObservableCollection<AccessTemplate>();
			AvailableAccessTemplates.Add(new AccessTemplate() { Name = "НЕТ" });
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
				OnPropertyChanged("Number");
			}
		}

		ObservableCollection<CardType> _cardTypes;
		public ObservableCollection<CardType> CardTypes
		{
			get { return _cardTypes; }
			set
			{
				_cardTypes = value;
				OnPropertyChanged("CardTypes");
			}
		}

		CardType _selectedCardType;
		public CardType SelectedCardType
		{
			get { return _selectedCardType; }
			set
			{
				_selectedCardType = value;
				OnPropertyChanged("SelectedCardType");
				OnPropertyChanged("CanSelectEndDate");

				if (value != CardType.Temporary)
				{
					EndDate = StartDate;
				}
			}
		}

		public bool CanSelectEndDate
		{
			get { return SelectedCardType == CardType.Temporary; }
		}

		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged("StartDate");

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
				OnPropertyChanged("EndDate");
			}
		}

		ObservableCollection<AccessTemplate> _availableAccessTemplates;
		public ObservableCollection<AccessTemplate> AvailableAccessTemplates
		{
			get { return _availableAccessTemplates; }
			set
			{
				_availableAccessTemplates = value;
				OnPropertyChanged("AvailableAccessTemplates");
			}
		}

		AccessTemplate _selectedAccessTemplate;
		public AccessTemplate SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				OnPropertyChanged("SelectedAccessTemplate");
			}
		}

		bool _useStopList;
		public bool UseStopList
		{
			get { return _useStopList; }
			set
			{
				_useStopList = value;
				OnPropertyChanged("UseStopList");
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
				OnPropertyChanged("SelectedStopListCard");
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
				OnPropertyChanged("UseReader");
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

		public RelayCommand ChangeReaderCommand { get; private set; }
		void OnChangeReader()
		{
			var readerSelectationViewModel = new ReaderSelectationViewModel(ClientSettings.SKDSettings.CardCreatorReaderUID);
			if (DialogService.ShowModalWindow(readerSelectationViewModel))
			{
				OnPropertyChanged("ReaderName");
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

			if (UseStopList && SelectedStopListCard != null)
			{
				if (!IsNewCard)
					CardHelper.DeleteFromEmployee(Card, "Заменен на пропуск " + Number);
				Card.UID = SelectedStopListCard.UID;
				Card.IsInStopList = false;
				Card.StopReason = null;
			}
			Card.Number = Number;
			Card.CardType = SelectedCardType;
			Card.StartDate = StartDate;
			Card.EndDate = EndDate;
			Card.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();

			if (SelectedAccessTemplate != null)
				Card.AccessTemplateUID = SelectedAccessTemplate.UID;
			if (AvailableAccessTemplates.IndexOf(SelectedAccessTemplate) == 0)
				Card.AccessTemplateUID = null;
			return true;
		}
	}
}