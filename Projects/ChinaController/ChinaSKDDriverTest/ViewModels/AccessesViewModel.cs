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
			
			InitAvailableAccessMethodTypes();
		}

	    private void InitAvailableAccessMethodTypes()
	    {
	        AvailableAccessMethodTypes = new ObservableCollection<AccessMethodType>
	        {
				AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_UNKNOWN,
				AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,
				AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_CARD,
				AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,
				AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,
				AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_REMOTE,
				AccessMethodType.NET_ACCESS_DOOROPEN_METHOD_BUTTON
	        };
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
			MessageBox.Show("Всего проходов: " + MainViewModel.Wrapper.GetAccessesCount());
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var accesses = MainViewModel.Wrapper.GetAllAccesses();

			Accesses.Clear();
			foreach (var access in accesses)
			{
				Accesses.Add(new AccessViewModel(access));
			}
		}

		Access GetModel()
		{
			var access = new Access
			{
				CardNo = CardNo,
				Password = Password,
				Time = Time,
				Status = Status,
				MethodType = MethodType,
				ReaderID = ReaderID
			};

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

		string _cardNo;
		/// <summary>
		/// Номер карты
		/// </summary>
		public string CardNo
		{
			get { return _cardNo; }
			set
			{
				_cardNo = value;
				OnPropertyChanged(() => CardNo);
			}
		}

		string _password;
		/// <summary>
		/// Пароль
		/// </summary>
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		DateTime _time;
		/// <summary>
		/// Время
		/// </summary>
		public DateTime Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged(() => Time);
			}
		}

		bool _status;
		/// <summary>
		/// Статус
		/// </summary>
		public bool Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged(() => Status);
			}
		}

		AccessMethodType _methodType;
		/// <summary>
		/// Метод доступа
		/// </summary>
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