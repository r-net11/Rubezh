using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using System;
using ControllerSDK.SDK;
using ControllerSDK.Views;
using ControllerSDK.API;
using System.Windows;
using System.Collections.Generic;

namespace ControllerSDK.ViewModels
{
	public class PasswordsViewModel  : BaseViewModel
	{
		public PasswordsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
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
			var newPasswordNo = SDKWrapper.AddPassword(MainWindow.LoginID, password);
			MessageBox.Show("newPasswordNo = " + newPasswordNo);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (SelectedPassword != null)
			{
				var result = SDKImport.WRAP_DevCtrl_RemoveRecordSet(MainWindow.LoginID, SelectedPassword.Password.RecordNo, 2);
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
			var passwordsCount = SDKImport.WRAP_DevCtrl_Get_Password_RecordSetCount(MainWindow.LoginID);
			MessageBox.Show("passwordsCount = " + passwordsCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var passwords = SDKWrapper.GetAllPasswords(MainWindow.LoginID);

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