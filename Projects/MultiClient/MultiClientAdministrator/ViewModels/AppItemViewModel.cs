using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using MuliclientAPI;

namespace MultiClient.ViewModels
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
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
				ShellViewModel.HasChanges = true;
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
				ShellViewModel.HasChanges = true;
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
				ShellViewModel.HasChanges = true;
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
				ShellViewModel.HasChanges = true;
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
				ShellViewModel.HasChanges = true;
			}
		}
	}
}