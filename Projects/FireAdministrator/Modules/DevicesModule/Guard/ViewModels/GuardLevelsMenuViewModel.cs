using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardLevelsMenuViewModel : BaseViewModel
    {
        public GuardLevelsMenuViewModel(GuardLevelsViewModel context)
        {
            Context = context;
        }

        public GuardLevelsViewModel Context { get; private set; }
    }
}
