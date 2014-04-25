using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation;
		public SKDCard Card { get; private set; }
		public AccessZonesSelectationViewModel AccessZones { get; private set; }
		bool IsNewCard;

		public EmployeeCardDetailsViewModel(Organisation organisation, SKDCard card = null)
		{
			ChangeReaderCommand = new RelayCommand(OnChangeReader);

			Organisation = organisation;
			Card = card;
			if (card == null)
			{
				IsNewCard = true;
				Title = "Создание карты";
				card = new SKDCard()
				{
					Series = 0,
					Number = 0,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddYears(1)
				};
			}
			else
			{
				Title = string.Format("Свойства карты: {0}", card.PresentationName);
			}
			Card = card;
			Series = Card.Series;
			Number = Card.Number;
			StartDate = Card.StartDate;
			EndDate = Card.EndDate;

			AccessZones = new AccessZonesSelectationViewModel(Organisation, Card.CardZones, Card.UID);

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

		int _series;
		public int Series
		{
			get { return _series; }
			set
			{
				_series = value;
				OnPropertyChanged("Series");
			}
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
				Series = SelectedStopListCard.Series;
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
					ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Subscribe(OnNewJournal);
				}
				else
				{
					ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Unsubscribe(OnNewJournal);
				}
			}
		}

		public void OnNewJournal(List<SKDJournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.DeviceUID == ClientSettings.SKDSettings.CardCreatorReaderUID)
				{
					if (journalItem.CardSeries > 0 && journalItem.CardNo > 0)
					{
						Series = journalItem.CardSeries;
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

		protected override bool Save()
		{
			if (EndDate < StartDate)
			{
				MessageBoxService.ShowWarning("Дата конца действия карты не может быть раньше даты начала действия");
				return false;
			}

			if (UseStopList && SelectedStopListCard != null)
			{
				if (!IsNewCard)
					CardHelper.ToStopList(Card, "Заменена на карту " + Series + @"\" + Number);
				Card.UID = SelectedStopListCard.UID;
				Card.IsInStopList = false;
				Card.StopReason = null;
			}
			Card.Series = Series;
			Card.Number = Number;
			Card.CardType = SelectedCardType;
			Card.StartDate = StartDate;
			Card.EndDate = EndDate;
			Card.CardZones = AccessZones.GetCardZones();

			if (SelectedAccessTemplate != null)
				Card.AccessTemplateUID = SelectedAccessTemplate.UID;
			if (AvailableAccessTemplates.IndexOf(SelectedAccessTemplate) == 0)
				Card.AccessTemplateUID = null;
			return true;
		}
	}
}