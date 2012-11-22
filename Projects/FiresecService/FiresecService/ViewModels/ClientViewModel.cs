using System;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

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
			if (IpAddress.StartsWith("127.0.0.1"))
				IpAddress = "localhost";
		}

        string _friendlyUserName;
        public string FriendlyUserName
        {
            get { return _friendlyUserName; }
            set
            {
                _friendlyUserName = value;
				OnPropertyChanged("FriendlyUserName");
            }
        }
    }
}