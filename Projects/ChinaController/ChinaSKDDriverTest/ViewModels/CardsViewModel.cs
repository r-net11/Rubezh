using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class CardsViewModel : BaseViewModel
	{
		public CardsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			AddManyCommand = new RelayCommand(OnAddMany);
			Cards = new ObservableCollection<CardViewModel>();

			CardNo = "1";
			Password = "1";
			UserTime = 1;
			ValidStartTime = DateTime.Now;
			ValidEndTime = DateTime.Now;

			AvailableCardTypes = new ObservableCollection<CardType>();
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_UNKNOWN);
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_GENERAL);
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_VIP);
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_GUEST);
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_PATROL);
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_BLACKLIST);
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_CORCE);
			AvailableCardTypes.Add(CardType.NET_ACCESSCTLCARD_TYPE_MOTHERCARD);

			Doors = new ObservableCollection<DoorItemViewModel>();
			for (int i = 0; i < 4; i++)
			{
				Doors.Add(new DoorItemViewModel(i));
			}
			Doors[0].IsChecked = true;

			TimeShedules = new ObservableCollection<TimeSheduleItemViewModel>();
			for (int i = 0; i < 10; i++)
			{
				TimeShedules.Add(new TimeSheduleItemViewModel(i));
			}
			TimeShedules[0].IsChecked = true;
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
			var card = GetModel();
			var newCardNo = MainViewModel.Wrapper.AddCard(card);
			MessageBox.Show("newCardNo = " + newCardNo);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var card = GetModel();
			var result = MainViewModel.Wrapper.EditCard(card);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var result = MainViewModel.Wrapper.RemoveCard(Index);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = MainViewModel.Wrapper.RemoveAllCards();
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetInfoCommand { get; private set; }
		void OnGetInfo()
		{
			var result = MainViewModel.Wrapper.GetCardInfo(Index);
			Initialize(new List<Card>() { result });
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var cardsCount = MainViewModel.Wrapper.GetCardsCount();
			MessageBox.Show("cardsCount = " + cardsCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var cards = MainViewModel.Wrapper.GetAllCards();

			Cards.Clear();
			foreach (var card in cards)
			{
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
			}
		}

		public RelayCommand AddManyCommand { get; private set; }
		void OnAddMany()
		{
			for (int i = 1; i <= 100; i++)
			{
				var card = GetModel();
				card.CardNo = (i*11).ToString();
				var newCardNo = MainViewModel.Wrapper.AddCard(card);
				Trace.WriteLine("Card Added " + card.CardNo);
			}
		}

		Card GetModel()
		{
			var card = new Card();
			card.CardNo = CardNo;
			card.Password = Password;
			card.UserTime = UserTime;
			card.ValidStartDateTime = ValidStartTime;
			card.ValidEndDateTime = ValidEndTime;
			card.CardType = CardType;

			foreach (var door in Doors)
			{
				if (door.IsChecked)
				{
					card.Doors.Add(door.No);
				}
			}
			card.DoorsCount = card.Doors.Count;

			foreach (var timeShedule in TimeShedules)
			{
				if (timeShedule.IsChecked)
				{
					card.TimeSections.Add(timeShedule.No);
				}
			}
			card.TimeSectionsCount = card.TimeSections.Count;
			return card;
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

		int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				_index = value;
				OnPropertyChanged("Index");
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

		public ObservableCollection<CardType> AvailableCardTypes { get; private set; }

		CardType _cardType;
		public CardType CardType
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

		public ObservableCollection<DoorItemViewModel> Doors { get; private set; }

		public ObservableCollection<TimeSheduleItemViewModel> TimeShedules { get; private set; }
	}
}