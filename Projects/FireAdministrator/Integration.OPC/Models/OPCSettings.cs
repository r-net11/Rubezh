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

		private string _opcAddress;
		public string OPCAddress
		{
			get { return _opcAddress; }
			set
			{
				if (string.Equals(_opcAddress, value)) return;
				_opcAddress = value;
				OnPropertyChanged(() => OPCAddress);
			}
		}

		private int _opcPort;
		public int OPCPort
		{
			get { return _opcPort; }
			set
			{
				if (_opcPort == value) return;
				_opcPort = value;
				OnPropertyChanged(() => OPCPort);
			}
		}

		private int _httpClientPort;

		public int HTTPClientPort
		{
			get { return _httpClientPort; }
			set
			{
				if (_httpClientPort == value) return;
				_httpClientPort = value;
				OnPropertyChanged(() => HTTPClientPort);
			}
		}

		public OPCSettings(StrazhAPI.Integration.OPC.OPCSettings settings)
		{
			if (settings == null) return;

			IsActive = settings.IsActive;
			OPCAddress = settings.OPCAddress;
			OPCPort = settings.OPCPort;
			HTTPClientPort = settings.HTTPClientPort;
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

				if (columnName == string.Empty || columnName == "OPCPort" || columnName == "HTTPClientPort")
				{
					if ((OPCPort < default(int)) || (HTTPClientPort < default(int)))
						result = "Значение порта не может быть отрицательным.";

					if ((OPCPort == default(int)) || (HTTPClientPort == default(int)))
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
				OPCAddress = OPCAddress,
				IsActive = IsActive,
				OPCPort = OPCPort,
				HTTPClientPort = HTTPClientPort
			};
		}
	}
}
