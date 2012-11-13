using System;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
    public class ClientViewModel : BaseViewModel
    {
		public FiresecService.Service.FiresecService FiresecService { get; private set; }
		public string ClientType { get; private set; }
		public Guid UID { get; private set; }
        public string IpAddress { get; private set; }

		public ClientViewModel(FiresecService.Service.FiresecService firesecService)
		{
			FiresecService = firesecService;
			ClientType = firesecService.ClientCredentials.ClientType.ToDescription();
			UID = firesecService.UID;
			UserName = firesecService.ClientCredentials.UserName;
			IpAddress = firesecService.ClientIpAddressAndPort;
			if (IpAddress.StartsWith("127.0.0.1"))
				IpAddress = "localhost";
		}

        string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }
    }
}