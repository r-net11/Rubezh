using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using MuliclientAPI;

namespace MultiClientAdministrator.ViewModels
{
	public class AppItemViewModel : BaseViewModel
	{
		public AppItemViewModel(MulticlientData multiclientData)
		{
			_name = multiclientData.Name;
			_address = multiclientData.Address;
			_port = multiclientData.Port;
			_login = multiclientData.Login;
			_password = multiclientData.Password;
			_isNotUsed = multiclientData.IsNotUsed;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged("Address");
			}
		}

		int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged("Port");
			}
		}

		string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged("Login");
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged("Password");
			}
		}

		bool _isNotUsed;
		public bool IsNotUsed
		{
			get { return _isNotUsed; }
			set
			{
				_isNotUsed = value;
				OnPropertyChanged("IsNotUsed");
			}
		}

		new void OnPropertyChanged(string propertyName)
		{
			base.OnPropertyChanged(propertyName);
			ShellViewModel.HasChanges = true;
		}
	}
}