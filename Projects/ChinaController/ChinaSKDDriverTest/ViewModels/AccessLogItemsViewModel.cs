using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ChinaSKDDriverAPI;
using ControllerSDK.Events;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AccessLogItemsViewModel : BaseViewModel
	{
		public AccessLogItemsViewModel()
		{
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			GenerateJournalItemCommand = new RelayCommand(OnGenerateJournalItem, CanGenerateJournalItem);
			AccessLogItems = new ObservableCollection<AccessLogItemViewModel>();

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

	    public void Initialize(List<AccessLogItem> accesses)
		{
			AccessLogItems.Clear();
			foreach (var access in accesses)
			{
				var accessViewModel = new AccessLogItemViewModel(access);
				AccessLogItems.Add(accessViewModel);
			}
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			MessageBox.Show("Всего проходов: " + MainViewModel.Wrapper.GetAccessLogItemsCount());
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var accesses = MainViewModel.Wrapper.GetAllAccessLogItems();

			AccessLogItems.Clear();
			foreach (var access in accesses)
			{
				AccessLogItems.Add(new AccessLogItemViewModel(access));
			}
		}

		public RelayCommand GenerateJournalItemCommand { get; private set; }

		private void OnGenerateJournalItem()
		{
			if (SelectedAccessLogItem == null)
				return;

			var journalItem = SelectedAccessLogItem.TransformToJournalItem();
			ServiceFactory.Instance.Events.GetEvent<JournalItemEvent>().Publish(journalItem);
		}

		private bool CanGenerateJournalItem()
		{
			return SelectedAccessLogItem != null;
		}

		AccessLogItem GetModel()
		{
			var access = new AccessLogItem
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

		public ObservableCollection<AccessLogItemViewModel> AccessLogItems { get; private set; }

		AccessLogItemViewModel _selectedAccessLogItem;
		public AccessLogItemViewModel SelectedAccessLogItem
		{
			get { return _selectedAccessLogItem; }
			set
			{
				_selectedAccessLogItem = value;
				OnPropertyChanged(() => SelectedAccessLogItem);
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