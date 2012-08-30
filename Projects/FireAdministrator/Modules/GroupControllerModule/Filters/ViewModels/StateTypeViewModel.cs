using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace GKModule.ViewModels
{
    public class StateTypeViewModel : BaseViewModel
    {
        public StateTypeViewModel(StateType stateType)
        {
            StateType = stateType;
        }

        public StateType StateType { get; private set; }
        public bool IsChecked { get; set; }
    }
}