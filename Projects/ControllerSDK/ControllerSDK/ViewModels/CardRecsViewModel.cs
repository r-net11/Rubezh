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
	public class CardRecsViewModel : BaseViewModel
	{
		public CardRecsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			Cards = new ObservableCollection<CardRecViewModel>();

			CreationDateTime = DateTime.Now;
			CardNo = "1";
			Password = "1";
			DoorNo = 1;
			IsStatus = true;

			AvailableDoorOpenMethods = new ObservableCollection<SDKImport.NET_ACCESS_DOOROPEN_METHOD>();
			AvailableDoorOpenMethods.Add(SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_UNKNOWN);
			AvailableDoorOpenMethods.Add(SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY);
			AvailableDoorOpenMethods.Add(SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD);
			AvailableDoorOpenMethods.Add(SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST);
			AvailableDoorOpenMethods.Add(SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST);
			AvailableDoorOpenMethods.Add(SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_REMOTE);
			AvailableDoorOpenMethods.Add(SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_BUTTON);
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
			var cardRec = new CardRec();
			cardRec.DateTime = CreationDateTime;
			cardRec.CardNo = CardNo;
			cardRec.Password = Password;
			cardRec.DoorNo = DoorNo;
			cardRec.IsStatus = IsStatus;
			cardRec.DoorOpenMethod = DoorOpenMethod;
			var newCardNo = SDKWrapper.AddCardRec(MainWindow.LoginID, cardRec);
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
				var result = SDKImport.WRAP_DevCtrl_RemoveRecordSet(MainWindow.LoginID, SelectedCard.CardRec.RecordNo, 2);
				MessageBox.Show("result = " + result);
			}
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = SDKImport.WRAP_DevCtrl_ClearRecordSet(MainWindow.LoginID, 2);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var cardsCount = SDKImport.WRAP_DevCtrl_Get_RecordSet_RecordSetCount(MainWindow.LoginID);
			MessageBox.Show("cardsRecCount = " + cardsCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var cardRecs = SDKWrapper.GetAllCardRecs(MainWindow.LoginID);

			Cards.Clear();
			foreach (var cardRec in cardRecs)
			{
				var cardViewModel = new CardRecViewModel(cardRec);
				Cards.Add(cardViewModel);
			}
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
				OnPropertyChanged(() => SelectedCard);
			}
		}

		public ObservableCollection<ControllerSDK.SDK.SDKImport.NET_ACCESS_DOOROPEN_METHOD> AvailableDoorOpenMethods { get; private set; }

		ControllerSDK.SDK.SDKImport.NET_ACCESS_DOOROPEN_METHOD _doorOpenMethod;
		public ControllerSDK.SDK.SDKImport.NET_ACCESS_DOOROPEN_METHOD DoorOpenMethod
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