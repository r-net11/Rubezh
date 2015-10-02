﻿using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public partial class ApartmentDetailsViewModel
	{
		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		//string _fio;
		//public string FIO
		//{
		//	get { return _fio; }
		//	set
		//	{
		//		_fio = value;
		//		OnPropertyChanged(() => FIO);
		//	}
		//}

		string _phone;
		public string Phone
		{
			get { return _phone; }
			set
			{
				_phone = value;
				OnPropertyChanged(() => Phone);
			}
		}

		string _email;
		public string Email
		{
			get { return _email; }
			set
			{
				_email = value;
				OnPropertyChanged(() => Email);
			}
		}

		string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		bool _isSendEmail;
		public bool IsSendEmail
		{
			get { return _isSendEmail; }
			set
			{
				_isSendEmail = value;
				OnPropertyChanged(() => IsSendEmail);
			}
		}
	}
}