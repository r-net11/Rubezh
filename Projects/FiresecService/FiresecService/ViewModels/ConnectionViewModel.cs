using System;
using Infrastructure.Common;

namespace FiresecService.ViewModels
{
    public class ConnectionViewModel : BaseViewModel
    {
        public Guid UID { get; set; }
        public string IpAddress { get; set; }
        public string ClientType { get; set; }
        public DateTime ConnectionDate { get; set; }

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