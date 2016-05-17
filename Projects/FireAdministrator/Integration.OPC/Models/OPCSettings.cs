using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace Integration.OPC.Models
{
	public class OPCSettings : BaseViewModel, IDataErrorInfo
	{

		public bool IsValid
		{
			get { return string.IsNullOrEmpty(Error); }
		}

		private bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive == value) return;
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}

		private string _ipAddress;
		public string IPAddress
		{
			get { return _ipAddress; }
			set
			{
				if (string.Equals(_ipAddress, value)) return;
				_ipAddress = value;
				OnPropertyChanged(() => IPAddress);
			}
		}

		private int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				if (_port == value) return;
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		public OPCSettings(StrazhAPI.Integration.OPC.OPCSettings settings)
		{
			if (settings == null) return;

			IsActive = settings.IsActive;
			IPAddress = settings.IPAddress;
			Port = settings.Port;
		}

		private string _currentError;
		public string CurrentError
		{
			get { return _currentError; }
			set
			{
				if (string.Equals(_currentError, value)) return;
				_currentError = value;
				OnPropertyChanged(() => CurrentError);
			}
		}

		public string Error { get { return this[string.Empty]; } }

		public string this[string columnName]
		{
			get
			{
				var result = string.Empty;
				columnName = columnName ?? string.Empty;

				if (columnName == string.Empty || columnName == "Port")
				{
					if (Port < default(int))
						result = "Значение порта не может быть отрицательным.";

					if (Port == default(int))
						result = "Значение порта не может быть равным нулю.";
				}

				CurrentError = result;
				return result;
			}
		}

		public StrazhAPI.Integration.OPC.OPCSettings ToDTO()
		{
			return new StrazhAPI.Integration.OPC.OPCSettings
			{
				IPAddress = IPAddress,
				IsActive = IsActive,
				Port = Port
			};
		}
	}
}
