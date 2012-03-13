using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupControllerModule.ViewModels
{
    public class DevicesMenuViewModel
    {
        public DevicesMenuViewModel(DevicesViewModel devicesViewModel)
        {
            Context = devicesViewModel;
        }

        public DevicesViewModel Context { get; private set; }
    }
}