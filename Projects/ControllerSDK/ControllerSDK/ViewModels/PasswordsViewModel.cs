using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using ControllerSDK.Views;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class PasswordsViewModel : BaseViewModel
	{
		public PasswordsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			Passwords = new ObservableCollection<PasswordViewModel>();

			CreationDateTime = DateTime.Now;
			UserID = "1";
			DoorOpenPassword = "1";
			AlarmPassword = "1";
			DoorsCount = 1;

		}

		public void Initialize(List<Password> passwords)
		{
			Passwords.Clear();
			foreach (var password in passwords)
			{
				var passwordViewModel = new PasswordViewModel(password);
				Passwords.Add(passwordViewModel);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var password = new Password();
			password.CreationDateTime = CreationDateTime;
			password.UserID = UserID;
			password.DoorOpenPassword = DoorOpenPassword;
			password.AlarmPassword = AlarmPassword;
			password.DoorsCount = DoorsCount;
			var newPasswordNo = Wrapper.AddPassword(MainWindow.LoginID, password);
			MessageBox.Show("newPasswordNo = " + newPasswordNo);
		}

		public RelayCommand GetInfoCommand { get; private set; }
		void OnGetInfo()
		{
			var result = Wrapper.GetPasswordInfo(MainWindow.LoginID, 0);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (SelectedPassword != null)
			{
				var result = NativeWrapper.WRAP_RemoveCardRec(MainWindow.LoginID, SelectedPassword.Password.RecordNo);
				MessageBox.Show("result = " + result);
			}
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = NativeWrapper.WRAP_RemoveAllPasswords(MainWindow.LoginID);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var passwordsCount = NativeWrapper.WRAP_Get_PasswordsCount(MainWindow.LoginID);
			MessageBox.Show("passwordsCount = " + passwordsCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var passwords = Wrapper.GetAllPasswords(MainWindow.LoginID);

			Passwords.Clear();
			foreach (var password in passwords)
			{
				var passwordViewModel = new PasswordViewModel(password);
				Passwords.Add(passwordViewModel);
			}
		}

		public ObservableCollection<PasswordViewModel> Passwords { get; private set; }

		PasswordViewModel _selectedPassword;
		public PasswordViewModel SelectedPassword
		{
			get { return _selectedPassword; }
			set
			{
				_selectedPassword = value;
				OnPropertyChanged(() => SelectedPassword);
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

		string _doorOpenPassword;
		public string DoorOpenPassword
		{
			get { return _doorOpenPassword; }
			set
			{
				_doorOpenPassword = value;
				OnPropertyChanged(() => DoorOpenPassword);
			}
		}

		string _alarmPassword;
		public string AlarmPassword
		{
			get { return _alarmPassword; }
			set
			{
				_alarmPassword = value;
				OnPropertyChanged(() => AlarmPassword);
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
	}
}