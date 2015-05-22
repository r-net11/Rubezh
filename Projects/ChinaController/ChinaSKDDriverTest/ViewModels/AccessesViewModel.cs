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
	public class AccessesViewModel : BaseViewModel
	{
		public AccessesViewModel()
		{
			//AddCommand = new RelayCommand(OnAdd);
			//EditCommand = new RelayCommand(OnEdit);
			//RemoveCommand = new RelayCommand(OnRemove);
			//RemoveAllCommand = new RelayCommand(OnRemoveAll);
			//GetInfoCommand = new RelayCommand(OnGetInfo);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			//AddManyCommand = new RelayCommand(OnAddMany);
			Accesses = new ObservableCollection<AccessViewModel>();

			CardNo = "";
			Password = "";
			Time = DateTime.Now;
			Status = false;
			MethodType = AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_UNKNOWN;
			ReaderID = -1;
			DoorNo = -1;
			
			AvailableAccessMethodTypes = new ObservableCollection<AccessMethodType>();
			AvailableAccessMethodTypes.Add(AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_UNKNOWN);
			AvailableAccessMethodTypes.Add(AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY);
			AvailableAccessMethodTypes.Add(AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_CARD);
			AvailableAccessMethodTypes.Add(AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST);
			AvailableAccessMethodTypes.Add(AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST);
			AvailableAccessMethodTypes.Add(AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_REMOTE);
			AvailableAccessMethodTypes.Add(AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_BUTTON);
		}

		public void Initialize(List<Access> accesses)
		{
			Accesses.Clear();
			foreach (var access in accesses)
			{
				var accessViewModel = new AccessViewModel(access);
				Accesses.Add(accessViewModel);
			}
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var accessesCount = MainViewModel.Wrapper.GetAccessesCount();
			MessageBox.Show("accessesCount = " + accessesCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var accesses = MainViewModel.Wrapper.GetAllAccesses();

			Accesses.Clear();
			foreach (var access in accesses)
			{
				var accessViewModel = new AccessViewModel(access);
				Accesses.Add(accessViewModel);
			}
		}

		Access GetModel()
		{
			var access = new Access();
			
			access.CardNo = CardNo;
			access.Password = Password;
			access.Time = Time;
			access.Status = Status;
			access.MethodType = MethodType;
			access.ReaderID = ReaderID;
			
			return access;
		}

		public ObservableCollection<AccessViewModel> Accesses { get; private set; }

		AccessViewModel _selectedAccess;
		public AccessViewModel SelectedAccess
		{
			get { return _selectedAccess; }
			set
			{
				_selectedAccess = value;
				OnPropertyChanged(() => SelectedAccess);
			}
		}

		/// <summary>
		/// Номер карты
		/// </summary>
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

		/// <summary>
		/// Пароль
		/// </summary>
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

		/// <summary>
		/// Время
		/// </summary>
		DateTime _time;
		public DateTime Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged(() => Time);
			}
		}

		/// <summary>
		/// Статус
		/// </summary>
		bool _status;
		public bool Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged(() => Status);
			}
		}

		/// <summary>
		/// Метод доступа
		/// </summary>
		AccessMethodType _methodType;
		public AccessMethodType MethodType
		{
			get { return _methodType; }
			set
			{
				_methodType = value;
				OnPropertyChanged(() => MethodType);
			}
		}

		int _readerID;
		public int ReaderID
		{
			get { return _readerID; }
			set
			{
				_readerID = value;
				OnPropertyChanged(() => ReaderID);
			}
		}

		int _doorNo;
		public int DoorNo
		{
			get	{ return _doorNo; }
			set
			{
				_doorNo = value;
				OnPropertyChanged(() => DoorNo);
			}
		}

		public ObservableCollection<AccessMethodType> AvailableAccessMethodTypes { get; private set; }

	}
}