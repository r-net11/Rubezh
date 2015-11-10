using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
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

			GkDevices = new ObservableCollection<GKDevice>(GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_GKMirror));

			if (user == null)
			{
				_isNew = true;
				Title = "Создание пользователя";
				var gkNo = (ushort)(parentViewModel.Users.Count > 0 ? parentViewModel.Users.Max(x => x.User.GkNo) + 1 : 1);
				var password = parentViewModel.Users.Count > 0 ? parentViewModel.Users.Max(x => x.User.Password) + 1 : 1;
				user = new GKUser(gkNo, GkDevices.FirstOrDefault()) 
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
				return new GKUser(GkNo, GkDevice)
				{
					Fio = Fio,
					Password = Password,
					UserType = UserType,
					ExpirationDate = ExpirationDate.DateTime
				};
			}
		}

		void CopyProperties(GKUser user)
		{
			GkNo = user.GkNo;
			Fio = user.Fio;
			Password = user.Password;
			UserType = user.UserType;
			ExpirationDate = new DateTimePairViewModel(user.ExpirationDate);
			GkDevice = GkDevices.FirstOrDefault(x => x.UID == user.GkDevice.UID);
		}
		
		ushort _gkNo;
		public ushort GkNo
		{
			get { return _gkNo; }
			set
			{
				_gkNo = value;
				OnPropertyChanged(() => GkNo);
			}
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

		public ObservableCollection<GKDevice> GkDevices { get; private set; }
		GKDevice _gkDevice;
		public GKDevice GkDevice
		{
			get { return _gkDevice; }
			set
			{
				_gkDevice = value;
				OnPropertyChanged(() => GkDevice);
			}
		}

		DateTimePairViewModel _expirationDate;
		public DateTimePairViewModel ExpirationDate
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
			if (GkDevice == null)
			{
				MessageBoxService.Show("Необходимо указать прибор, к которому будет добавлен пользователь");
				return false;
			}
			if(_isNew || (GkNo != _oldUser.GkNo))
			{
				if(_parentViewModel.Users.Any(x => x.User.GkNo == GkNo && x.User.GkDevice.UID == GkDevice.UID))
				{
					MessageBoxService.Show("Невозможно добавить пользователя с совпадающим номером");
					return false;
				}
			}
			if (_isNew || (Password != _oldUser.Password))
			{
				if (_parentViewModel.Users.Any(x => x.User.Password == Password && x.User.GkDevice.UID == GkDevice.UID))
				{
					MessageBoxService.Show("Невозможно добавить пользователя с совпадающим паролем");
					return false;
				}
			}
			_isSaved = true;
			if (!_isNew)
				GKManager.PmfUsers.Remove(_oldUser);
			GKManager.PmfUsers.Add(User);
			ServiceFactory.SaveService.GKChanged = true;
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
