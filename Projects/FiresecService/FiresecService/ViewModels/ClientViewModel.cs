using System;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
    public class ClientViewModel : BaseViewModel
    {
        public FiresecService.Service.FiresecService FiresecService { get; set; }
        public Guid UID { get; set; }
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