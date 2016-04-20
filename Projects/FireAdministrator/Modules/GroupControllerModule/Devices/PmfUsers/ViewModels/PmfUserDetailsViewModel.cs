using Infrastructure;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class PmfUserDetailsViewModel : SaveCancelDialogViewModel
	{
		PmfUsersViewModel _parentViewModel;
		bool _isNew;
		GKUser _oldUser;
		bool _isSaved;
		bool _isCancelClosing;
		
		public PmfUserDetailsViewModel(PmfUsersViewModel parentViewModel, GKUser user = null)
		{
			_parentViewModel = parentViewModel;
			UserTypes = new ObservableCollection<GKCardType>(Enum.GetValues(typeof(GKCardType)).OfType<GKCardType>());
			UserTypes.Remove(GKCardType.Employee);

			if (user == null)
			{
				_isNew = true;
				Title = "Создание пользователя";
				var password = parentViewModel.Users.Count > 0 ? parentViewModel.Users.Max(x => x.User.Password) + 1 : 1;
				user = new GKUser 
				{
					Password = password, 
					ExpirationDate = DateTime.Now.AddYears(1), 
					Fio = "Новый пользователь", 
					UserType = GKCardType.Operator 
				};
			}
			else
			{
				_oldUser = user;
				Title = "Редактирование пользователя " + user.Fio;
			}
			CopyProperties(user);
		}

		public GKUser User
		{
			get
			{
				return new GKUser
				{
					Fio = Fio,
					Password = Password,
					UserType = UserType,
					ExpirationDate = ExpirationDate
				};
			}
		}

		void CopyProperties(GKUser user)
		{
			Fio = user.Fio;
			Password = user.Password;
			UserType = user.UserType;
			ExpirationDate = user.ExpirationDate;
		}
		
		string _fio;
		public string Fio
		{
			get { return _fio; }
			set
			{
				_fio = value;
				OnPropertyChanged(() => Fio);
			}
		}

		uint _password;
		public uint Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		public ObservableCollection<GKCardType> UserTypes { get; private set; }
		GKCardType _userType;
		public GKCardType UserType
		{
			get { return _userType; }
			set
			{
				_userType = value;
				OnPropertyChanged(() => UserType);
			}
		}

		DateTime _expirationDate;
		public DateTime ExpirationDate
		{
			get { return _expirationDate; }
			set
			{
				_expirationDate = value;
				OnPropertyChanged(() => ExpirationDate);
			}
		}

		protected override bool Save()
		{
			if (_isNew || (Password != _oldUser.Password))
			{
				if (_parentViewModel.Users.Any(x => x.User.Password == Password))
				{
					MessageBoxService.Show("Невозможно добавить пользователя с совпадающим паролем");
					return false;
				}
			}
			_isSaved = true;
			return base.Save();
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (_isSaved)
				return base.OnClosing(isCanceled);
			if (!_isCancelClosing)
			{
				var isConfirmed = MessageBoxService.ShowConfirmation("Вы уверены, что хотите закрыть окно без сохранения изменений?");
				if (!isConfirmed)
				{
					_isCancelClosing = true;
					return true;
				}
			}
			else
			{
				_isCancelClosing = false;
				return true;
			}
			return base.OnClosing(isCanceled);
		}
	}

	public class DateTimePairViewModel : BaseViewModel
	{
		public DateTimePairViewModel(DateTime dateTime)
		{
			Date = dateTime.Date;
			Time = dateTime.TimeOfDay;
		}

		DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged(() => Date);
			}
		}

		TimeSpan _time;
		public TimeSpan Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged(() => Time);
			}
		}

		public DateTime DateTime
		{
			get { return new DateTime(Date.Year, Date.Month, Date.Day, Time.Hours, Time.Minutes, Time.Seconds); }
		}
	}
}
