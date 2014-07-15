using Controls;
using FiresecAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class CodeDetailsViewModel : SaveCancelDialogViewModel
	{
		public XCode Code { get; private set; }

		public CodeDetailsViewModel(XCode code = null)
		{
			if (code == null)
			{
				Title = "Создать код";
				Code = new XCode();
			}
			else
			{
				Title = "Редактировать код";
				Code = code;
			}

			CopyProperies();
		}

		void CopyProperies()
		{
			Name = Code.Name;
			Password = Code.Password;
			CanSetZone = Code.CanSetZone;
			CanUnSetZone = Code.CanUnSetZone;
		}

		void SaveProperies()
		{
			Code.Name = Name;
			Code.Password = Password;
			Code.CanSetZone = CanSetZone;
			Code.CanUnSetZone = CanUnSetZone;
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
				OnPropertyChanged("Name");
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
				OnPropertyChanged("Password");
			}
		}

		bool _canSetZone;
		public bool CanSetZone
		{
			get { return _canSetZone; }
			set
			{
				_canSetZone = value;
				OnPropertyChanged("CanSetZone");
			}
		}

		bool _canUnSetZone;
		public bool CanUnSetZone
		{
			get { return _canUnSetZone; }
			set
			{
				_canUnSetZone = value;
				OnPropertyChanged("CanUnSetZone");
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