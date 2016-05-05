using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using StrazhDeviceSDK.API;

namespace ControllerSDK.ViewModels
{
	public class PasswordsViewModel : BaseViewModel
	{
		public PasswordsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			Passwords = new ObservableCollection<PasswordViewModel>();

			CreationDateTime = DateTime.Now;
			UserID = "1";
			DoorOpenPassword = "123456";
			AlarmPassword = "123456";

			Doors = new ObservableCollection<DoorItemViewModel>();
			for (int i = 0; i < 4; i++)
			{
				Doors.Add(new DoorItemViewModel(i));
			}
			Doors[0].IsChecked = true;
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
			var password = GetModel();
			var newPasswordNo = MainViewModel.Wrapper.AddPassword(password);
			MessageBox.Show("newPasswordNo = " + newPasswordNo);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var password = GetModel();
			var result = MainViewModel.Wrapper.EditPassword(password);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var result = MainViewModel.Wrapper.RemovePassword(Index);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = MainViewModel.Wrapper.RemoveAllPasswords();
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetInfoCommand { get; private set; }
		void OnGetInfo()
		{
			var result = MainViewModel.Wrapper.GetPasswordInfo(Index);
			Initialize(new List<Password>() { result });
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var passwordsCount = MainViewModel.Wrapper.GetPasswordsCount();
			MessageBox.Show("passwordsCount = " + passwordsCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var passwords = MainViewModel.Wrapper.GetAllPasswords();

			Passwords.Clear();
			foreach (var password in passwords)
			{
				var passwordViewModel = new PasswordViewModel(password);
				Passwords.Add(passwordViewModel);
			}
		}

		Password GetModel()
		{
			var password = new Password();
			password.UserID = UserID;
			password.CreationDateTime = CreationDateTime;
			password.DoorOpenPassword = DoorOpenPassword;
			password.AlarmPassword = AlarmPassword;
			password.DoorsCount = Doors.Count;

			foreach (var door in Doors)
			{
				if (door.IsChecked)
				{
					password.Doors.Add(door.No);
				}
			}
			password.DoorsCount = password.Doors.Count;
			return password;
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

		int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				_index = value;
				OnPropertyChanged(() => Index);
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

		public ObservableCollection<DoorItemViewModel> Doors { get; private set; }
	}
}