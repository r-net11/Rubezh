using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;
using FiresecAPI.GK;
using System.Threading;

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation;
		public SKDCard Card { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }
		public bool IsNewCard { get; private set; }
		public bool HasGK { get; private set; }
		public bool HasSKD { get; private set; }
		ShortEmployee _employee;
		
		public EmployeeCardDetailsViewModel(Organisation organisation, ShortEmployee employee, SKDCard card = null)
		{
			HasGK = GKManager.Devices.Count > 1;
			HasSKD = SKDManager.Devices.Count > 1;
			_employee = employee;

			ChangeDeactivationControllerCommand = new RelayCommand(OnChangeDeactivationController);
			ChangeReaderCommand = new RelayCommand(OnChangeReader);
			ShowUSBCardReaderCommand = new RelayCommand(OnShowUSBCardReader);

			Organisation = OrganisationHelper.GetSingle(organisation.UID);
			Card = card;
			if (card == null)
			{
				IsNewCard = true;
				Title = "Создание пропуска";
				card = new SKDCard()
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
			GKLevel = Card.GKLevel;
			UserTime = Card.UserTime;
			DeactivationControllerUID = Card.DeactivationControllerUID;

			GKSchedules = new ObservableCollection<GKSchedule>();
			foreach(var schedule in GKManager.DeviceConfiguration.Schedules)
			{
				GKSchedules.Add(schedule);
			}
			SelectedGKSchedule = GKSchedules.FirstOrDefault(x => x.No == Card.GKLevelSchedule);

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
			if (_employee.Type == PersonType.Guest)
			{
				CardTypes.Remove(CardType.Constant);
				CardTypes.Remove(CardType.Duress);
			}
			SelectedCardType = Card.CardType;
		}

		uint _number;
		public uint Number
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

				if (!CanSelectEndDate)
				{
					EndDate = StartDate;
				}
			}
		}

		public bool CanSelectEndDate
		{
			get { return SelectedCardType == CardType.Temporary || SelectedCardType == CardType.Duress || !HasSKD; }
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

		int _gkLevel;
		public int GKLevel
		{
			get { return _gkLevel; }
			set
			{
				_gkLevel = value;
				OnPropertyChanged(() => GKLevel);
			}
		}

		public ObservableCollection<GKSchedule> GKSchedules { get; private set; }

		GKSchedule _selectedGKSchedule;
		public GKSchedule SelectedGKSchedule
		{
			get { return _selectedGKSchedule; }
			set
			{
				_selectedGKSchedule = value;
				OnPropertyChanged(() => SelectedGKSchedule);
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
			get { return HasSKD && SelectedCardType == CardType.OneTime; }
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
				if (HasGK)
				{
					if (value)
					{
						StartPollThread();
					}
					else
					{
						StopPollThread();
					}
				}
				{
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
		}

		void StartPollThread()
		{
			StopPollThread();
			IsThreadPolling = true;
			PollReaderThread = new Thread(OnPollReader);
			PollReaderThread.Start();
		}

		void StopPollThread()
		{
			IsThreadPolling = false;
			if (PollReaderThread != null)
			{
				PollReaderThread.Join(TimeSpan.FromSeconds(2));
			}
		}

		Thread PollReaderThread;
		bool IsThreadPolling = false;
		void OnPollReader()
		{
			var readerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == ClientSettings.SKDSettings.CardCreatorReaderUID);
			if (readerDevice != null)
			{
				while (IsThreadPolling)
				{
					var operationResult = FiresecManager.FiresecService.GKGetReaderCode(readerDevice);
					if (!operationResult.HasError && operationResult.Result > 0)
					{
						Number = operationResult.Result;
					}
					Thread.Sleep(TimeSpan.FromMilliseconds(500));
				}
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
						Number = (uint)cardNo;
					}
				}
			}
		}

		public RelayCommand ChangeReaderCommand { get; private set; }
		void OnChangeReader()
		{
			if (HasGK)
			{
				var readerSelectationViewModel = new GKReaderSelectationViewModel(ClientSettings.SKDSettings.CardCreatorReaderUID);
				if (DialogService.ShowModalWindow(readerSelectationViewModel))
				{
					OnPropertyChanged(() => ReaderName);
					UseReader = UseReader;
				}
			}
			else
			{
				var readerSelectationViewModel = new ReaderSelectationViewModel(ClientSettings.SKDSettings.CardCreatorReaderUID);
				if (DialogService.ShowModalWindow(readerSelectationViewModel))
				{
					OnPropertyChanged(() => ReaderName);
				}
			}
		}

		public string ReaderName
		{
			get
			{
				if (HasGK)
				{
					var readerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == ClientSettings.SKDSettings.CardCreatorReaderUID);
					if (readerDevice != null)
					{
						return readerDevice.PresentationName;
					}
					else
					{
						return "Нажмите для выбора считывателя";
					}
				}
				else
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
			if (SelectedCardType == CardType.Temporary || SelectedCardType == CardType.Duress)
			{
				if (EndDate < DateTime.Now)
				{
					MessageBoxService.ShowWarning("Дата конца действия пропуска не может быть меньше теущей даты");
					return false;
				}
			}

			if (EndDate < StartDate)
			{
				MessageBoxService.ShowWarning("Дата конца действия пропуска не может быть раньше даты начала действия");
				return false;
			}

			if(Number <= 0)
			{
				MessageBoxService.ShowWarning("Номер карты должен быть задан в пределах 1 ... 2147483647");
				return false;
			}

			if (SelectedCardType == CardType.OneTime && DeactivationControllerUID != Guid.Empty && UserTime <= 0)
			{
				MessageBoxService.ShowWarning("Количество проходов для разого пропуска должно быть задано в пределах от 1 до " + Int16.MaxValue.ToString());
				return false;
			}

			if (!string.IsNullOrEmpty(Password))
			{
				foreach (char c in Password)
				{
					if (!Char.IsDigit(c))
					{
						MessageBoxService.ShowWarning("Пароль может содержать только цифры");
						return false;
					}
				}
			}

			if (GKLevel < 0 || GKLevel > 255)
			{
				MessageBoxService.ShowWarning("Уровень доступа должен быть в пределах от 0 до 255");
				return false;
			}

			Card.Number = Number;
			var stopListCard = StopListCards.FirstOrDefault(x => x.Number == Card.Number);
			if (stopListCard != null)
			{
				if (MessageBoxService.ShowQuestion("Карта с таким номеромнаходится в стоп-листе. Использовать её?"))
				{
					UseStopList = true;
					SelectedStopListCard = stopListCard;
				}
				else
				{
					return false;
				}
			}
	
			if (UseStopList && SelectedStopListCard != null)
			{
				Card.UID = SelectedStopListCard.UID;
				Card.IsInStopList = false;
				Card.StopReason = null;
			}
			
			Card.Password = Password;
			Card.CardType = SelectedCardType;
			Card.StartDate = StartDate;
			Card.EndDate = EndDate;
			Card.GKLevel = GKLevel;
			if (SelectedGKSchedule != null)
			{
				Card.GKLevelSchedule = SelectedGKSchedule.No;
			}
			Card.UserTime = UserTime;
			Card.DeactivationControllerUID = DeactivationControllerUID;
			Card.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();
			Card.CardDoors.ForEach(x => x.CardUID = Card.UID);
			Card.OrganisationUID = Organisation.UID;
			Card.HolderUID = _employee.UID;
			Card.EmployeeName = _employee.Name;

			if (SelectedAccessTemplate != null)
				Card.AccessTemplateUID = SelectedAccessTemplate.UID;
			if (AvailableAccessTemplates.IndexOf(SelectedAccessTemplate) == 0)
				Card.AccessTemplateUID = null;
			if (!Validate())
				return false;
			var saveResult = IsNewCard ? CardHelper.Add(Card) : CardHelper.Edit(Card);
			if (!saveResult)
				return false;
			ServiceFactory.Events.GetEvent<NewCardEvent>().Publish(Card);
			return true;
		}

		public override void OnClosed()
		{
			StopPollThread();
			base.OnClosed();
		}

		bool Validate()
		{
			if (Card.Password != null && Card.Password.Length > 50)
			{
				MessageBoxService.Show("Значение поля 'Пароль' не может быть длиннее 50 символов");
				return false;
			}
			return true;
		}

		bool CheckChar(char c)
		{
			switch(c)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case 'a':
				case 'A':
				case 'b':
				case 'B':
				case 'c':
				case 'C':
				case 'd':
				case 'D':
				case 'e':
				case 'E':
				case 'f':
				case 'F':
					return true;
			}
			return false;
		}
	}
}