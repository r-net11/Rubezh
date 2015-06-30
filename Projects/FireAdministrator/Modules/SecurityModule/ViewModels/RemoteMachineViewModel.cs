﻿using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class RemoteMachineViewModel : SaveCancelDialogViewModel
	{
		public RemoteMachineViewModel()
		{
			Title = "Задайте имя или адрес компьютера";
			IsDnsName = true;
		}

		string _hostNameOrAddress;
		public string HostNameOrAddress
		{
			get { return _hostNameOrAddress; }
			set
			{
				_hostNameOrAddress = value;
				OnPropertyChanged(() => HostNameOrAddress);
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
					HostNameOrAddress = string.Empty;
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
					HostNameOrAddress = string.Empty;
					IsIpAddress = false;
				}

				OnPropertyChanged(() => IsDnsName);
			}
		}
	}
}