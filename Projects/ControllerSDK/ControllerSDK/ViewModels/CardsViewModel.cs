using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ControllerSDK.API;
using ControllerSDK.SDK;
using ControllerSDK.Views;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class CardsViewModel : BaseViewModel
	{
		public CardsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			Cards = new ObservableCollection<CardViewModel>();

			CreationDateTime = DateTime.Now;
			CardNo = "1";
			UserID = "1";
			Password = "1";
			DoorsCount = 1;
			TimeSectionsCount = 1;
			UserTime = 1;
			ValidStartTime = DateTime.Now;
			ValidEndTime = DateTime.Now;
			IsValid = true;

			AvailableCardStatuses = new ObservableCollection<SDKImport.NET_ACCESSCTLCARD_STATE>();
			AvailableCardStatuses.Add(SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_FREEZE);
			AvailableCardStatuses.Add(SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_NORMAL);
			AvailableCardStatuses.Add(SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_LOSE);
			AvailableCardStatuses.Add(SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_LOGOFF);
			AvailableCardStatuses.Add(SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_FREEZE);
			AvailableCardTypes = new ObservableCollection<SDKImport.NET_ACCESSCTLCARD_TYPE>();
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_UNKNOWN);
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_GENERAL);
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_VIP);
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_GUEST);
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_PATROL);
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_BLACKLIST);
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_CORCE);
			AvailableCardTypes.Add(SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_MOTHERCARD);
		}

		public void Initialize(List<Card> cards)
		{
			Cards.Clear();
			foreach (var card in cards)
			{
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var card = new Card();
			card.CreationDateTime = CreationDateTime;
			card.CardNo = CardNo;
			card.UserID = UserID;
			card.Password = Password;
			card.DoorsCount = DoorsCount;
			card.TimeSectionsCount = TimeSectionsCount;
			card.UserTime = UserTime;
			card.ValidStartDateTime = ValidStartTime;
			card.ValidEndDateTime = ValidEndTime;
			card.IsValid = IsValid;
			card.CardStatus = CardStatus;
			card.CardType = CardType;
			var newCardNo = SDKWrapper.AddCard(MainWindow.LoginID, card);
			MessageBox.Show("newCardNo = " + newCardNo);
		}

		public RelayCommand GetInfoCommand { get; private set; }
		void OnGetInfo()
		{
			var result = SDKWrapper.GetCardInfo(MainWindow.LoginID, 2);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (SelectedCard != null)
			{
				var result = SDKImport.WRAP_DevCtrl_RemoveRecordSet(MainWindow.LoginID, SelectedCard.Card.RecordNo, 1);
				MessageBox.Show("result = " + result);
			}
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = SDKImport.WRAP_DevCtrl_ClearRecordSet(MainWindow.LoginID, 1);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			SDKImport.FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam = new SDKImport.FIND_RECORD_ACCESSCTLCARD_CONDITION();
			stuParam.szCardNo = SDKWrapper.StringToCharArray("1", 32);
			stuParam.szUserID = SDKWrapper.StringToCharArray("1", 32);
			var cardsCount = SDKImport.WRAP_Get_CardsCount(MainWindow.LoginID, ref stuParam);
			MessageBox.Show("cardsCount = " + cardsCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var cards = SDKWrapper.GetAllCards(MainWindow.LoginID);

			Cards.Clear();
			foreach (var card in cards)
			{
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
			}
		}

		public ObservableCollection<CardViewModel> Cards { get; private set; }

		CardViewModel _selectedCard;
		public CardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged(() => SelectedCard);
			}
		}

		DateTime _creationDateTime;
		public DateTime CreationDateTime
		{
			get { return _creationDateTime; }
			set
			{
				_creationDateTime = value;
				OnPropertyChanged(() => CreationDateTime);
			}
		}

		string _cardNo;
		public string CardNo
		{
			get { return _cardNo; }
			set
			{
				_cardNo = value;
				OnPropertyChanged(() => CardNo);
			}
		}

		string _userID;
		public string UserID
		{
			get { return _userID; }
			set
			{
				_userID = value;
				OnPropertyChanged(() => UserID);
			}
		}

		public ObservableCollection<ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_STATE> AvailableCardStatuses { get; private set; }

		ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_STATE _cardStatus;
		public ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_STATE CardStatus
		{
			get { return _cardStatus; }
			set
			{
				_cardStatus = value;
				OnPropertyChanged(() => CardStatus);
			}
		}

		public ObservableCollection<ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_TYPE> AvailableCardTypes { get; private set; }

		ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_TYPE _cardType;
		public ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_TYPE CardType
		{
			get { return _cardType; }
			set
			{
				_cardType = value;
				OnPropertyChanged(() => CardType);
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

		int _doorsCount;
		public int DoorsCount
		{
			get { return _doorsCount; }
			set
			{
				_doorsCount = value;
				OnPropertyChanged(() => DoorsCount);
			}
		}

		int _timeSectionsCount;
		public int TimeSectionsCount
		{
			get { return _timeSectionsCount; }
			set
			{
				_timeSectionsCount = value;
				OnPropertyChanged(() => TimeSectionsCount);
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

		DateTime _validStartTime;
		public DateTime ValidStartTime
		{
			get { return _validStartTime; }
			set
			{
				_validStartTime = value;
				OnPropertyChanged(() => ValidStartTime);
			}
		}

		DateTime _validEndTime;
		public DateTime ValidEndTime
		{
			get { return _validEndTime; }
			set
			{
				_validEndTime = value;
				OnPropertyChanged(() => ValidEndTime);
			}
		}

		bool _isValid;
		public bool IsValid
		{
			get { return _isValid; }
			set
			{
				_isValid = value;
				OnPropertyChanged(() => SelectedCard);
			}
		}
	}
}