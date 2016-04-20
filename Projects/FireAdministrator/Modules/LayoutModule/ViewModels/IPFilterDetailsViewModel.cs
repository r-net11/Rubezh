using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Text.RegularExpressions;

namespace LayoutModule.ViewModels
{
	public class IPFilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public IPFilterDetailsViewModel()
		{
			Title = "Задайте имя или адрес компьютера";
			IsDnsName = true;
		}

		string _hostAddress;
		public string HostAddress
		{
			get { return _hostAddress; }
			set
			{
				_hostAddress = value;
				OnPropertyChanged(() => HostAddress);
			}
		}

		string _hostName;
		public string HostName
		{
			get { return _hostName; }
			set
			{
				_hostName = value;
				OnPropertyChanged(() => HostName);
			}
		}
		public string HostNameOrAddress
		{
			get
			{
				if (_isDnsName)
					return _hostName;
				else
					return _hostAddress;
			}
		}

		bool _isIpAddress;
		public bool IsIpAddress
		{
			get { return _isIpAddress; }
			set
			{
				_isIpAddress = value;
				if (_isIpAddress)
				{
					HostName = string.Empty;
					IsDnsName = false;
				}

				OnPropertyChanged(() => IsIpAddress);
			}
		}

		bool _isDnsName;
		public bool IsDnsName
		{
			get { return _isDnsName; }
			set
			{
				_isDnsName = value;
				if (_isDnsName)
				{
					HostAddress = string.Empty;
					IsIpAddress = false;
				}

				OnPropertyChanged(() => IsDnsName);
			}
		}
		protected override bool Save()
		{
			if (IsIpAddress && !IsCorrectIP(HostNameOrAddress) && !string.IsNullOrEmpty(HostNameOrAddress))
			{
				ServiceFactory.MessageBoxService.ShowWarning("Не корректный IP адрес");
				return false;
			}

			return base.Save();
		}
		bool IsCorrectIP(string address)
		{
			const string pattern = @"^([01]\d\d?|[01]?[1-9]\d?|2[0-4]\d|25[0-3])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$";
			if (!Regex.IsMatch(address, pattern))
			{
				return false;
			}
			return true;
		}
	}
}