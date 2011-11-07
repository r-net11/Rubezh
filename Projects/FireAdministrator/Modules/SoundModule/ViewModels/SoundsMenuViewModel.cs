using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    public class SoundsMenuViewModel : BaseViewModel
    {
        public SoundsMenuViewModel(SoundsViewModel context)
        {
            Context = context;
        }

        public SoundsViewModel Context { get; private set; }
    }
}
