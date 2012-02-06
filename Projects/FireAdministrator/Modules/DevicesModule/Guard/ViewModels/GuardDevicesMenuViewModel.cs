using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    class GuardDevicesMenuViewModel : BaseViewModel
    {
        public GuardDevicesMenuViewModel(GuardDevicesViewModel context)
        {
            Context = context;
        }

        public GuardDevicesViewModel Context { get; private set; }
    }
}
