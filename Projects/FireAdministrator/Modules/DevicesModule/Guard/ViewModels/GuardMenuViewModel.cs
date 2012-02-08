using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    class GuardMenuViewModel : BaseViewModel
    {
        public GuardMenuViewModel(GuardViewModel context)
        {
            Context = context;
        }

        public GuardViewModel Context { get; private set; }
    }
}
