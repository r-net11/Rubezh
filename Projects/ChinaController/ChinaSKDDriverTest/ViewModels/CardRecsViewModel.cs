using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class CardRecsViewModel : BaseViewModel
	{
		public CardRecsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			Cards = new ObservableCollection<CardRecViewModel>();

			CreationDateTime = DateTime.Now;
			CardNo = "1";
			Password = "1";
			DoorNo = 1;
			IsStatus = true;

			AvailableDoorOpenMethods = new ObservableCollection<CardRecDoorOpenMethod>();
			AvailableDoorOpenMethods.Add(CardRecDoorOpenMethod.NET_ACCESS_DOOROPEN_METHOD_UNKNOWN);
			AvailableDoorOpenMethods.Add(CardRecDoorOpenMethod.NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY);
			AvailableDoorOpenMethods.Add(CardRecDoorOpenMethod.NET_ACCESS_DOOROPEN_METHOD_CARD);
			AvailableDoorOpenMethods.Add(CardRecDoorOpenMethod.NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST);
			AvailableDoorOpenMethods.Add(CardRecDoorOpenMethod.NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST);
			AvailableDoorOpenMethods.Add(CardRecDoorOpenMethod.NET_ACCESS_DOOROPEN_METHOD_REMOTE);
			AvailableDoorOpenMethods.Add(CardRecDoorOpenMethod.NET_ACCESS_DOOROPEN_METHOD_BUTTON);
		}

		public void Initialize(List<CardRec> cardRecs)
		{
			Cards.Clear();
			foreach (var cardRec in cardRecs)
			{
				var cardViewModel = new CardRecViewModel(cardRec);
				Cards.Add(cardViewModel);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var cardRec = GetModel();
			cardRec.DoorOpenMethod = DoorOpenMethod;
			var newCardNo = MainViewModel.Wrapper.AddCardRec(cardRec);
			MessageBox.Show("newCardNo = " + newCardNo);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var cardRec = GetModel();
			var result = MainViewModel.Wrapper.EditCardRec(cardRec);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var result = MainViewModel.Wrapper.RemoveCardRec(Index);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = MainViewModel.Wrapper.RemoveAllCardRecs();
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetInfoCommand { get; private set; }
		void OnGetInfo()
		{
			var result = MainViewModel.Wrapper.GetCardRecInfo(Index);
			Initialize(new List<CardRec>() { result });
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var cardsCount = MainViewModel.Wrapper.GetCardRecsCount();
			MessageBox.Show("cardsRecCount = " + cardsCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var cardRecs = MainViewModel.Wrapper.GetAllCardRecs();

			Cards.Clear();
			foreach (var cardRec in cardRecs)
			{
				var cardViewModel = new CardRecViewModel(cardRec);
				Cards.Add(cardViewModel);
			}
		}

		CardRec GetModel()
		{
			var cardRec = new CardRec();
			cardRec.DateTime = CreationDateTime;
			cardRec.CardNo = CardNo;
			cardRec.Password = Password;
			cardRec.DoorNo = DoorNo;
			cardRec.IsStatus = IsStatus;
			cardRec.DoorOpenMethod = DoorOpenMethod;
			return cardRec;
		}

		public ObservableCollection<CardRecViewModel> Cards { get; private set; }

		CardRecViewModel _selectedCard;
		public CardRecViewModel SelectedCard
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

		int _doorNo;
		public int DoorNo
		{
			get { return _doorNo; }
			set
			{
				_doorNo = value;
				OnPropertyChanged(() => DoorNo);
			}
		}

		bool _isStatus;
		public bool IsStatus
		{
			get { return _isStatus; }
			set
			{
				_isStatus = value;
				OnPropertyChanged(() => IsStatus);
			}
		}

		public ObservableCollection<CardRecDoorOpenMethod> AvailableDoorOpenMethods { get; private set; }

		CardRecDoorOpenMethod _doorOpenMethod;
		public CardRecDoorOpenMethod DoorOpenMethod
		{
			get { return _doorOpenMethod; }
			set
			{
				_doorOpenMethod = value;
				OnPropertyChanged(() => DoorOpenMethod);
			}
		}
	}
}