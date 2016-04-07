using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class RemoteMachineViewModel : SaveCancelDialogViewModel
	{
		public RemoteMachineViewModel()
		{
			Title = "Задайте имя или адрес компьютера";
			IsDnsName = true;
		}
		
		string _hostAddress;
		string _hostName;
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
		public string HostName
		{
			get { return _hostName; }
			set
			{
				_hostName = value;
				OnPropertyChanged(() => HostName);
			}
		}
		public string HostAddress
		{
			get { return _hostAddress; }
			set
			{
				_hostAddress = value;
				OnPropertyChanged(() => HostAddress);
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
	}
}