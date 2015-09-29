﻿using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation;
		public SKDCard Card { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }
		public bool IsFirstRadioButtonChecked { get; set; }
		public bool IsAlternativeLockParams { get; set; }
		public bool IsNewCard { get; private set; }
		ShortEmployee _employee;

		private bool _isEnableFromDeactivationCard;

		public bool IsEnableFromDeactivationCard
		{
			get { return _isEnableFromDeactivationCard; }
			set
			{
				_isEnableFromDeactivationCard = value;
				OnPropertyChanged(() => IsEnableFromDeactivationCard);
			}
		}

		private bool _enableValidationForUSB;

		public bool EnableValidationForUSB
		{
			get { return _enableValidationForUSB; }
			set
			{
				_enableValidationForUSB = value;
				OnPropertyChanged(() => EnableValidationForUSB);
			}
		}

		public EmployeeCardDetailsViewModel(Organisation organisation, ShortEmployee employee, SKDCard card = null)
		{
			_employee = employee;

			ChangeDeactivationControllerCommand = new RelayCommand(OnChangeDeactivationController);
			ChangeReaderCommand = new RelayCommand(OnChangeReader);
			ShowUSBCardReaderCommand = new RelayCommand(OnShowUSBCardReader);

			Organisation = OrganisationHelper.GetSingle(organisation.UID);
			Card = card;

			if (card == null)
			{
				IsNewCard = true;
				IsFirstRadioButtonChecked = true;
				Title = "Создание пропуска";
				card = new SKDCard
				{
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

			AvailableAccessTemplates = new ObservableCollection<AccessTemplate> {new AccessTemplate {Name = "<нет>"}};
			var accessTemplateFilter = new AccessTemplateFilter();
			accessTemplateFilter.OrganisationUIDs.Add(Organisation.UID);
			var accessTemplates = AccessTemplateHelper.Get(accessTemplateFilter);
			if (accessTemplates != null)
			{
				foreach (var accessTemplate in accessTemplates)
					AvailableAccessTemplates.Add(accessTemplate);
			}

			SelectedAccessTemplate = AvailableAccessTemplates.FirstOrDefault(x => x.UID == Card.AccessTemplateUID);
			var cards = CardHelper.Get(new CardFilter());
			Cards = cards != null ? new ObservableCollection<SKDCard>(cards) : new ObservableCollection<SKDCard>();

			StopListCards = new ObservableCollection<SKDCard>();
			foreach (var item in cards.Where(x => x.IsInStopList))
				StopListCards.Add(item);
			SelectedStopListCard = StopListCards.FirstOrDefault();

			if (_employee.Type == PersonType.Guest)
			{
				CardTypes = new ObservableCollection<CardType> { CardType.Temporary, CardType.OneTime, CardType.Blocked };
			}
			else
			{
				CardTypes = new ObservableCollection<CardType>(Enum.GetValues(typeof(CardType)).OfType<CardType>());
			}
			SelectedCardType = CardTypes.FirstOrDefault();
		}

		uint? _number;
		public uint? Number
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

		public ObservableCollection<CardType> CardTypes { get; private set; }

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

				if (!CanSelectEndDate)
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
			//get { return HasSKD && SelectedCardType == CardType.OneTime; }
			get { return false; }
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
				Number = 0;
				UpdateStopListCard();
			}
		}

		public ObservableCollection<SKDCard> Cards { get; private set; }
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
					ServiceFactoryBase.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournal);
				}
				else
				{
					ServiceFactoryBase.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournal);
				}
			}
		}

		private uint? _numberFromControlReader;

		public uint? NumberFromControlReader
		{
			get { return _numberFromControlReader; }
			set
			{
				_numberFromControlReader = value;
				OnPropertyChanged(() => NumberFromControlReader);
			}
		}

		public void OnNewJournal(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.ObjectUID == ClientSettings.SKDSettings.CardCreatorReaderUID)
				{
					var journalDetalisationItem = journalItem.JournalDetalisationItems.FirstOrDefault(x => x.Name == "Номер карты");
					if (journalDetalisationItem != null)
					{
						var cardNoString = journalDetalisationItem.Value;
						int cardNo;
						Int32.TryParse(cardNoString, System.Globalization.NumberStyles.HexNumber, null, out cardNo);
						NumberFromControlReader = (uint)cardNo;
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
				IsReaderSelected = true;
				OnPropertyChanged(() => ReaderName);
			}
		}

		private bool _isReaderSelected;

		public bool IsReaderSelected
		{
			get { return _isReaderSelected; }
			set
			{
				if (_isReaderSelected == value) return;
				_isReaderSelected = value;
				OnPropertyChanged(() => IsReaderSelected);
			}
		}

		public string ReaderName
		{
			get { return "Нажмите для выбора считывателя"; }
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
				return controllerDevice != null ? controllerDevice.Name : "Нажмите для выбора контроллера";
			}
		}

		public RelayCommand ShowUSBCardReaderCommand { get; private set; }
		void OnShowUSBCardReader()
		{
			var cardNumberViewModel = new CardNumberViewModel();
			if (DialogService.ShowModalWindow(cardNumberViewModel))
			{
				Number = (uint)cardNumberViewModel.Number;
			}
		}

		protected override bool Save()
		{
			var manualInputValidationCondition = (IsFirstRadioButtonChecked || EnableValidationForUSB) && Number == null;
			var useReaderValidationCondition = UseReader && NumberFromControlReader == null;

			if (manualInputValidationCondition || useReaderValidationCondition)
			{
				MessageBoxService.ShowError("Введите номер карты", "Неверный номер карты");
				return false;
			}

			Card = CopyCardData(IsEnableFromDeactivationCard);

			if (!Validate())
				return false;

			var saveResult = IsNewCard ? CardHelper.Add(Card, _employee.Name) : CardHelper.Edit(Card, _employee.Name);
			if (!saveResult)
				return false;

			ServiceFactoryBase.Events.GetEvent<NewCardEvent>().Publish(Card);
			return true;
		}

		private SKDCard CopyCardData(bool useFromDeactivate)
		{
			var resultCard = new SKDCard();

			if (useFromDeactivate)
			{
				resultCard.UID = SelectedStopListCard.UID;
				resultCard.Number = SelectedStopListCard.Number;
				resultCard.IsInStopList = false;
				resultCard.StopReason = null;
			}
			else if (UseReader)
			{
				resultCard.Number = NumberFromControlReader;
			}
			else
			{
				resultCard.Number = Number;

				if (!IsNewCard)
					resultCard.UID = Card.UID;
			}

			resultCard.IsHandicappedCard = IsAlternativeLockParams;
			resultCard.Password = Password;
			resultCard.CardType = SelectedCardType;
			resultCard.StartDate = StartDate;
			resultCard.EndDate = EndDate;
			resultCard.UserTime = UserTime;
			resultCard.DeactivationControllerUID = DeactivationControllerUID;
			resultCard.DeactivationControllerUID = DeactivationControllerUID;
			resultCard.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();
			resultCard.CardDoors.ForEach(x => x.CardUID = Card.UID);
			resultCard.OrganisationUID = Organisation.UID;
			resultCard.HolderUID = _employee.UID;
			resultCard.EmployeeName = _employee.Name;

			if (SelectedAccessTemplate != null)
				resultCard.AccessTemplateUID = SelectedAccessTemplate.UID;
			if (AvailableAccessTemplates.IndexOf(SelectedAccessTemplate) == 0)
				resultCard.AccessTemplateUID = null;

			return resultCard;
		}

		private bool Validate()
		{
			if (SelectedCardType == CardType.Temporary || SelectedCardType == CardType.Duress)
			{
				if (EndDate.Date < DateTime.Now.Date)
				{
					MessageBoxService.ShowWarning("Дата конца действия пропуска не может быть меньше текущей даты");
					return false;
				}
			}

			if (EndDate < StartDate)
			{
				MessageBoxService.ShowWarning("Дата конца действия пропуска не может быть раньше даты начала действия");
				return false;
			}

			if ((EnableValidationForUSB || IsFirstRadioButtonChecked) && (Number <= 0 || Number > Int32.MaxValue))
			{
				MessageBoxService.ShowWarning("Номер карты должен быть задан в пределах 1 ... 2147483647");
				return false;
			}

			if(Cards.Any(x => x.Number == Card.Number && x.UID != Card.UID))
			{
				MessageBoxService.ShowWarning("Невозможно добавить карту с повторяющимся номером");
				return false;
			}

			if (SelectedCardType == CardType.OneTime && DeactivationControllerUID != Guid.Empty && UserTime <= 0)
			{
				MessageBoxService.ShowWarning("Количество проходов для разого пропуска должно быть задано в пределах от 1 до " + Int16.MaxValue);
				return false;
			}

			if (string.IsNullOrEmpty(Password)) return true;
			if (Password.All(Char.IsDigit)) return true;

			MessageBoxService.ShowWarning("Пароль может содержать только цифры");
			return false;
		}
	}
}