using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class GuardUserViewModel : BaseViewModel
    {
        public GuardUserViewModel(GuardUser guardUser)
        {
            GuardUser = guardUser;
        }

        public GuardUser GuardUser { get; private set; }

        public void Update()
        {
            OnPropertyChanged("GuardUser");
        }
    }
}
