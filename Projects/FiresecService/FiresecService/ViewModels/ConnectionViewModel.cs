using System;
using Infrastructure.Common;

namespace FiresecService.ViewModels
{
    public class ConnectionViewModel : BaseViewModel
    {
        public Guid FiresecServiceUID { get; set; }
        public string IpAddress { get; set; }

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
