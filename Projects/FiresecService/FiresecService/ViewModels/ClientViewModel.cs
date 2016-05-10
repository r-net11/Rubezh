using StrazhAPI;
using StrazhAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace FiresecService.ViewModels
{
	public class ClientViewModel : BaseViewModel
	{
		public ClientCredentials ClientCredentials { get; private set; }

		public string ClientType { get; private set; }

		public Guid UID { get; private set; }

		public string IpAddress { get; private set; }

		public ClientViewModel(ClientCredentials clientCredentials)
		{
			ClientCredentials = clientCredentials;
			ClientType = clientCredentials.ClientType.ToDescription();
			UID = ClientCredentials.ClientUID;
			FriendlyUserName = clientCredentials.FriendlyUserName;
			IpAddress = clientCredentials.ClientIpAddressAndPort;
			if (IpAddress.StartsWith(NetworkHelper.LocalhostIp))
				IpAddress = NetworkHelper.Localhost;
		}

		private string _friendlyUserName;

		public string FriendlyUserName
		{
			get { return _friendlyUserName; }
			set
			{
				_friendlyUserName = value;
				OnPropertyChanged("FriendlyUserName");
			}
		}

		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if (_isChecked == value)
					return;
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}