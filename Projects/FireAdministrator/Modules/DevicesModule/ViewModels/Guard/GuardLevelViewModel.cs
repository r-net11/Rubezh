using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class GuardLevelViewModel : BaseViewModel
    {
        public GuardLevelViewModel(GuardLevel guardLevel)
        {
            GuardLevel = guardLevel;
        }

        GuardLevel _guardLevel;
        public GuardLevel GuardLevel
        {
            get { return _guardLevel; }
            set
            {
                _guardLevel = value;
                OnPropertyChanged("GuardLevel");
            }
        }
    }
}
