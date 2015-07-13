using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using FiresecAPI.GK;
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

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation;
		public SKDCard Card { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }
		public bool IsNewCard { get; private set; }
		public bool CanChangeCardType { get; private set; }
		ShortEmployee _employee;

		public EmployeeCardDetailsViewModel(Organisation organisation, ShortEmployee employee, SKDCard card = null)
		{
			_employee = employee;

			ChangeReaderCommand = new RelayCommand(OnChangeReader);
			ShowUSBCardReaderCommand = new RelayCommand(OnShowUSBCardReader);

			Organisation = OrganisationHelper.GetSingle(organisation.UID);
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
			CopyProperties();
			InitializeGKSchedules();
			AccessDoorsSelectationViewModel = new AccessDoorsSelectationViewModel(Organisation, Card.CardDoors, GKSchedules);
			InitializeAccessTemplates();

			var cards = CardHelper.Get(new CardFilter() { DeactivationType = LogicalDeletationType.Deleted });

			StopListCards = new ObservableCollection<SKDCard>();
			foreach (var item in cards.Where(x => x.IsInStopList))
				StopListCards.Add(item);
			SelectedStopListCard = StopListCards.FirstOrDefault();

			if (_employee.Type == PersonType.Guest)
			{
				CardTypes = new ObservableCollection<CardType>() { CardType.Temporary, CardType.OneTime, CardType.Blocked };
			}
			else
			{
				CardTypes = new ObservableCollection<CardType>(Enum.GetValues(typeof(CardType)).OfType<CardType>());
			}
			SelectedCardType = Card.CardType;
			CanChangeCardType = _employee.Type == PersonType.Employee && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_Employees_Edit_CardType);
			GKCardTypes = new ObservableCollection<GKCardType>(Enum.GetValues(typeof(GKCardType)).OfType<GKCardType>());
			SelectedGKCardType = Card.GKCardType;

			AvailableGKControllers = new ObservableCollection<GKControllerViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.GK)
				{
					var controllerViewModel = new GKControllerViewModel(device);
					controllerViewModel.IsChecked = IsNewCard || (card.GKControllerUIDs != null && card.GKControllerUIDs.Contains(device.UID));
					AvailableGKControllers.Add(controllerViewModel);
				}
			}
		}

		private void CopyProperties()
		{
			Number = Card.Number;
			Password = Card.Password;
			StartDate = Card.StartDate;
			EndDate = Card.EndDate;
			GKLevel = Card.GKLevel;
			UserTime = Card.UserTime;
		}

		private void InitializeAccessTemplates()
		{
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
		}

		private void InitializeGKSchedules()
		{
			GKSchedules = new ObservableCollection<GKSchedule>();
			var scheduleModels = GKScheduleHelper.GetSchedules();
			if (scheduleModels == null)
				scheduleModels = new List<GKSchedule>();
			foreach (var schedule in scheduleModels)
			{
				GKSchedules.Add(schedule);
			}
			SelectedGKSchedule = GKSchedules.FirstOrDefault(x => x.No == Card.GKLevelSchedule);
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

		public ObservableCollection<CardType> CardTypes { get; private set; }

		CardType _selectedCardType;
		public CardType SelectedCardType
		{
			get { return _selectedCardType; }
			set
			{
				_selectedCardType = value;
				OnPropertyChanged(() => SelectedCardType);
				OnPropertyChanged(() => CanSetUserTime);
			}
		}

		public ObservableCollection<GKCardType> GKCardTypes { get; private set; }

		GKCardType _selectedGKCardType;
		public GKCardType SelectedGKCardType
		{
			get { return _selectedGKCardType; }
			set
			{
				_selectedGKCardType = value;
				OnPropertyChanged(() => SelectedGKCardType);
				CanSelectGKControllers = value != GKCardType.Employee;
			}
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
			//get { return HasSKD && SelectedCardType == CardType.OneTime; }
			get { return false; }
		}

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

				if (value)
				{
					StartPollThread();
				}
				else
				{
					StopPollThread();
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

		public RelayCommand ChangeReaderCommand { get; private set; }
		void OnChangeReader()
		{
			var readerSelectationViewModel = new GKReaderSelectationViewModel(ClientSettings.SKDSettings.CardCreatorReaderUID);
			if (DialogService.ShowModalWindow(readerSelectationViewModel))
			{
				OnPropertyChanged(() => ReaderName);
				UseReader = UseReader;
			}
		}

		public string ReaderName
		{
			get
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

		#region GKControllers

		public ObservableCollection<GKControllerViewModel> AvailableGKControllers { get; private set; }

		bool _canSelectGKControllers;
		public bool CanSelectGKControllers
		{
			get { return _canSelectGKControllers; }
			set
			{
				_canSelectGKControllers = value;
				OnPropertyChanged(() => CanSelectGKControllers);
			}
		}

		#endregion

		protected override bool Save()
		{
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
			Card.GKCardType = SelectedGKCardType;
			if (SelectedGKCardType != GKCardType.Employee)
			{
				Card.GKControllerUIDs = AvailableGKControllers.Where(x => x.IsChecked).Select(x => x.Device.UID).ToList();
			}
			Card.StartDate = StartDate;
			Card.EndDate = EndDate;
			Card.GKLevel = GKLevel;
			if (SelectedGKSchedule != null)
			{
				Card.GKLevelSchedule = SelectedGKSchedule.No;
			}
			Card.UserTime = UserTime;
			Card.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();
			Card.CardDoors.ForEach(x => x.CardUID = Card.UID);
			Card.OrganisationUID = Organisation.UID;
			Card.EmployeeUID = _employee.UID;
			Card.EmployeeName = _employee.Name;

			if (SelectedAccessTemplate != null)
				Card.AccessTemplateUID = SelectedAccessTemplate.UID;
			if (AvailableAccessTemplates.IndexOf(SelectedAccessTemplate) == 0)
				Card.AccessTemplateUID = null;
			if (!Validate())
				return false;
			var saveResult = IsNewCard ? CardHelper.Add(Card, _employee.Name) : CardHelper.Edit(Card, _employee.Name);
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

			if (Number <= 0 || Number > Int32.MaxValue)
			{
				MessageBoxService.ShowWarning("Номер карты должен быть задан в пределах 1 ... 2147483647");
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
			return true;
		}
	}
}