using System.Linq;
using Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class UserDetailsViewModel : SaveCancelDialogViewModel
	{
		public GuardUser GuardUser { get; private set; }

		public UserDetailsViewModel(GuardUser guardUser = null)
		{
			if (guardUser == null)
			{
				Title = "Создать пользователя";
				GuardUser = new GuardUser()
				{
					Id = 1
				};

				if (FiresecManager.GuardUsers.Count != 0)
					GuardUser.Id = FiresecManager.GuardUsers.Select(x => x.Id).Max() + 1;
			}
			else
			{
				Title = "Редактировать пользователя";
				GuardUser = guardUser;
			}

			CopyProperies();
		}

		void CopyProperies()
		{
			Name = GuardUser.Name;
			Password = GuardUser.Password;
			FIO = GuardUser.FIO;
			Function = GuardUser.Function;
			CanSetZone = GuardUser.CanSetZone;
			CanUnSetZone = GuardUser.CanUnSetZone;
		}

		void SaveProperies()
		{
			GuardUser.Name = Name;
			GuardUser.Password = Password;
			GuardUser.FIO = FIO;
			GuardUser.Function = Function;
			GuardUser.CanSetZone = CanSetZone;
			GuardUser.CanUnSetZone = CanUnSetZone;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				if ((_name != null) && (_name.Length > 20))
					_name = _name.Substring(0, 20);
				OnPropertyChanged(() => Name);
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				if ((_password != null) && (_password.Length > 6))
					_password = _password.Substring(0, 6);
				OnPropertyChanged(() => Password);
			}
		}

		string _FIO;
		public string FIO
		{
			get { return _FIO; }
			set
			{
				_FIO = value;
				OnPropertyChanged(() => FIO);
			}
		}

		string _function;
		public string Function
		{
			get { return _function; }
			set
			{
				_function = value;
				OnPropertyChanged(() => Function);
			}
		}

		bool _canSetZone;
		public bool CanSetZone
		{
			get { return _canSetZone; }
			set
			{
				_canSetZone = value;
				OnPropertyChanged(() => CanSetZone);
			}
		}

		bool _canUnSetZone;
		public bool CanUnSetZone
		{
			get { return _canUnSetZone; }
			set
			{
				_canUnSetZone = value;
				OnPropertyChanged(() => CanUnSetZone);
			}
		}

		protected override bool Save()
		{
			if (!string.IsNullOrEmpty(Password))
				if (!DigitalPasswordHelper.Check(Password))
				{
					MessageBoxService.Show("Пароль может содержать только цифры");
					return false;
				}

			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.Show("Имя не может быть пустым");
				return false;
			}

			SaveProperies();
			return base.Save();
		}
	}
}